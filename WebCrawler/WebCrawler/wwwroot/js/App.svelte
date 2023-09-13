<svelte:options tag="svelte-app" />

<script>
    import NodeGraph from "./NodeGraph.svelte";
    
    const metaDataUri = '/Api/GetMetaData';
    const fullDataUri = '/Api/GetFullData';
    const latestExecutionUri = '/Api/GetLatestExecutions'
    const formUri = '/Home/AddRecord'

    // TODO We could possibly update this interval dynamicaly
    const graphUpdateInterval = 5000;
    const executionUpdateInterval = 10000; //10 seconds

    let currentRecordIndex = 0;
    let currentExecutionIndex = 0;
    let metaData;
    let currentRecordFullData;
    let currentRecordDomainData;

    let modeButton;
    let staticMode = false;
    let viewButton;
    let websiteView = true;

    let websiteGraph;
    let domainGraph;

    let regexInput = document.getElementById("regex");
    let form = document.getElementById("WebRecordForm");

    getData();
    setInterval(() => updateExecutionInformationInRecordTable(), executionUpdateInterval);

    form.addEventListener("submit", (event) => {
        let regex;
        try {
            regex = new RegExp(regexInput.value);
            regexInput.setCustomValidity("");
        }
        catch(e) {
            regexInput.setCustomValidity("Invalid Regular Expression");
        }
    });

    // Data retrieval
    function getData() {
        getMetaData().then(data => metaData = data);
        getFullData(currentRecordIndex).then(data => {
            currentRecordFullData = JSON.parse(data);
            if (currentRecordFullData["executions"] == undefined) {
                return;
            }

            currentRecordDomainData = getDomainData(currentRecordFullData["executions"][currentExecutionIndex]);

            if (websiteView && websiteGraph != null && websiteGraph !== undefined) {
                websiteGraph.updateData(currentRecordFullData["executions"][currentExecutionIndex]); 
                return;
            }

            if (!websiteView && domainGraph != null && domainGraph !== undefined) {
                domainGraph.updateData(currentRecordDomainData);
                return;
            }
        });

        if (!staticMode) {
            setTimeout(getData, graphUpdateInterval);
        }
    }

    function getMetaData() {
        return fetch(metaDataUri)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get metaData.', error));
    }

    function getFullData(id) {
        return fetch(fullDataUri + "/?recordId=" + id)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error("Unable to getFullData for recordId" + id + ".", error));
    }

    function updateExecutionInformationInRecordTable() {
        fetch(latestExecutionUri + "/")
            .then(res => {
                if (res.ok) {
                    return res.json()
                } else {
                    throw new Error("Unable to fetch latest executions")
                }
            })
            .then(json => JSON.parse(json))
            .then(jsonData => {
                jsonData["Executions"].forEach(execution => {
                    let timeDOM = document.getElementById("ExecutionTime" + execution["RecordId"])
                    let statusDOM = document.getElementById("ExecutionStatus" + execution["RecordId"])
                    timeDOM.innerHTML = execution["Time"];
                    statusDOM.innerHTML = execution["Status"];
                })
            })
            .catch(err => console.error(err));
    }

    function getDomainData(websiteData) {
        let websiteNodes = websiteData["nodes"];
        let websiteLinks = websiteData["links"];
        let domainNodes = getDomainNodes(websiteNodes);
        let domainLinks = getDomainLinks(websiteLinks);

        return {"nodes": domainNodes, "links": domainLinks};
    }

    function getDomainLinks(websiteLinks) {
        let domainLinks = [];
        for (let i = 0; i < websiteLinks.length; i++) {
            const source = getDomain(websiteLinks[i].source);
            const target = getDomain(websiteLinks[i].target);

            let findDomainLink = domainLinks.find((domainLink) => {
                domainLink.source === source &&
                domainLink.target === target
            });

            if (findDomainLink === undefined && source != target) {
                domainLinks[domainLinks.length] = {
                    "source": source,
                    "target": target,
                    "value": 1
                }
            }
        }

        return domainLinks;
    }

    function getDomainNodes(websiteNodes) {

        let domainNodes = [];
        for (let i = 0; i < websiteNodes.length; i++) {
            let nodeDomain = getDomain(websiteNodes[i].id);
            let nodeMatchIndex = domainNodes.findIndex((node) => node.id === nodeDomain);

            const websiteNodeMatch = websiteNodes[i]["match"];

            if (nodeMatchIndex === -1) {
                domainNodes[domainNodes.length] = {
                    "id": nodeDomain, 
                    "title": "",
                    "crawl-time": websiteNodes[i]["crawl-time"],
                    "crawled-by": websiteNodes[i]["crawled-by"],
                    "group": domainNodes.length,
                    "match": websiteNodeMatch
                }
            }
            else {
                let websiteCrawlers = websiteNodes[i]["crawled-by"];
                let domainCrawlers = domainNodes[nodeMatchIndex]["crawled-by"];

                websiteCrawlers.forEach((websiteCrawlerUrl) => {
                    let domainCrawler = domainCrawlers.find((crawler) => crawler === websiteCrawlerUrl);
                    if (domainCrawler === undefined) {
                        domainCrawlers[domainCrawlers.length] = websiteCrawlerUrl;
                    }
                });

                domainNodes[nodeMatchIndex]["crawled-by"] = domainCrawlers;

                if (domainNodes[nodeMatchIndex]["match"] == "false" && websiteNodeMatch == "true") {
                    domainNodes[nodeMatchIndex]["match"] = websiteNodeMatch;
                }
            }
        }

        return domainNodes;
    }

    function getDomain(urlString) {
        let url;
        try {
            url = new URL(urlString);
            return url.hostname;
        }
        catch(e) {
            console.log("Error", e);
            return url;
        }
    }

    function switchGraphMode() {
        if (staticMode) {
            modeButton.textContent = "Make Static";
            staticMode = false;
            getData();
        }
        else {
            modeButton.textContent = "Make Active";
            staticMode = true;
        }
    }

    function updateDomainGraph() {
        if (domainGraph !== undefined && domainGraph !== null && currentRecordDomainData !== undefined) {
            domainGraph.updateData(currentRecordDomainData);
        }
        else {
            setTimeout(updateDomainGraph, 500);
        }
    }

    function updateWebsiteGraph() {
        if (websiteGraph !== undefined && websiteGraph !== null && currentRecordFullData["executions"] !== undefined) {
            websiteGraph.updateData(currentRecordFullData["executions"][currentExecutionIndex]);
        }
        else {
            setTimeout(updateWebsiteGraph, 500);
        }
    }

    function switchGraphView() {
        if (websiteView) {
            viewButton.textContent = "View Websites"
            websiteView = false;
            if (staticMode) {
                updateDomainGraph();
            }
        }
        else {
            viewButton.textContent = "View Domains"
            websiteView = true;
            if (staticMode) {
                updateWebsiteGraph();
            }
        }
    }

    const buttonStyle = "display: inline-block;font-weight: 400;line-height: 1.5;color: #212529;text-align: center;text-decoration: none;vertical-align: middle;cursor: pointer;user-select: none;background-color: transparent;border: 1px solid transparent;padding: 0.375rem 0.75rem;font-size: 1rem;border-radius: 0.25rem;transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;color: #fff;background-color: #6c757d;border-color: #6c757d;";
  
</script>

<button style={buttonStyle} class="btn btn-primary" bind:this={modeButton} on:click={switchGraphMode}>Make Static</button>
<button style={buttonStyle} class="btn btn-primary" bind:this={viewButton} on:click={switchGraphView}>View Domains</button>

<!-- TODO Could be only one NodeGraph with changing data for performace reasons -->
{#if websiteView}
    <NodeGraph bind:this={websiteGraph}></NodeGraph>
{:else}
    <NodeGraph bind:this={domainGraph}></NodeGraph>
{/if}