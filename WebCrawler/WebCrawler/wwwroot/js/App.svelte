<svelte:options tag="svelte-app" />

<script>
    import NodeGraph from "./NodeGraph.svelte";
    import WebRecordTable from "./WebRecordTable.svelte";
    import ExecutionsTable from "./ExecutionsTable.svelte";

    // Enum of Views 
    class View {
        static Website = new View('Website');
        static Domain = new View('Domain');

        constructor(name) {
            this.name = name;
        }
        toString() {
            return `View.${this.name}`;
        }
    }

    const fullDataUri = '/Api/GetFullData';
    const webRecordsDataUri =  "/Api/GetWebsiteRecords";
    const startNewExecutionUri = "/Api/StartNewExecution";
    const deleteWebSiteRecordUri = "/Api/DeleteWebSiteRecord";
    const executionsDataUri = "./Api/GetExecutions/";
    const formUri = "/Home/AddRecord";
    const multiGraphUri = "/Api/GetGraphByIds";

    // Intervals in ms
    const graphUpdateInterval = 5000;
    const webRecordUpdateInterval = 1000;
    const executionUpdateInterval = 1000; 

    let currentRecordIndex = 0;
    let currentExecutionIndex = 0;
    let currentRecordFullData;
    let currentRecordDomainData;

    let modeButton;
    let staticGraphMode = false;
    let viewButton;
    let graphView = View.Website;

    let nodeGraph;
    let webRecordTable;
    let executionsTable;

    let regexInput = document.getElementById("regex");
    let form = document.getElementById("WebRecordForm");


    getGraphData(false);
    getWebRecordData();
    setInterval(getWebRecordData, webRecordUpdateInterval);
    setInterval(getExecutionsData, executionUpdateInterval);

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
    function getGraphData(recordChange) {
        getFullData(currentRecordIndex).then(data => {
            currentRecordFullData = JSON.parse(data);
            if (currentRecordFullData["executions"] == undefined) {
                return;
            }

            currentRecordDomainData = getDomainData(currentRecordFullData["executions"][currentExecutionIndex]);
            
            updateNodeGraph(recordChange, graphView);
        });

        if (!staticGraphMode && !recordChange) {
            setTimeout(getGraphData, graphUpdateInterval);
        }
    }

    function getExecutionsData() {
        fetch(executionsDataUri)
        .then(result => {
        if (result.ok) {
            return result.json()
        } else {
            throw new Error("Unable to fetch executions.")
        }
        })
        .then(json => {
            let jsonData = JSON.parse(json);
            if (jsonData["Executions"] == undefined || executionsTable == null || executionsTable == undefined) {
                return;
            }

            executionsTable.update(jsonData["Executions"]);
        })
    }

    function getWebRecordData() {
        fetch(webRecordsDataUri + "/")
        .then(res => {
            if (res.ok) {
                return res.json()
            } else {
                throw new Error("Unable to fetch WebRecords.")
            }
        })
        .then(json => JSON.parse(json))
        .then(jsonData => {
            if (jsonData["WebsiteRecords"] == undefined || webRecordTable == null || webRecordTable == undefined) {
                return;
            }

            webRecordTable.update(jsonData["WebsiteRecords"]);
        })
    }

    function startNewExecution(recordId) {
        fetch(startNewExecutionUri + "/?recordId=" + recordId);
    }


    function deleteWebSiteRecord(recordId) {
        fetch(deleteWebSiteRecordUri + "/?recordId=" + recordId , { method: 'DELETE' });
    }

    function getFullData(id) {
        return fetch(fullDataUri + "/?recordId=" + id)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error("Unable to getFullData for recordId" + id + ".", error));
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

    function filterExecutions(id) {
        if (executionsTable == null || executionsTable == undefined) {
            return;
        }

        executionsTable.filterExecutionsById(id);
    }

    function switchGraphMode() {
        if (staticGraphMode) {
            modeButton.textContent = "Make Static";
            staticGraphMode = false;
            getGraphData();
        }
        else {
            modeButton.textContent = "Make Active";
            staticGraphMode = true;
        }
    }

    function updateNodeGraph(switchView, view) {
        let newData = view === View.Domain ? currentRecordDomainData : currentRecordFullData["executions"][currentExecutionIndex];
        console.log(newData);
        if (nodeGraph === undefined || nodeGraph === null || newData === undefined) {
            // Try it again in 0.5s
            setTimeout(() => updateNodeGraph(switchView, view), 500);
        }

        if (switchView) {
            nodeGraph.clearData();
        }

        nodeGraph.updateData(newData);
    }

    function switchGraphView() {
        if (graphView === View.Website) {
            viewButton.textContent = "View Websites";
            graphView = View.Domain;

            updateNodeGraph(true, graphView);
        }
        else {
            viewButton.textContent = "View Domains";
            graphView = View.Website;
        
            updateNodeGraph(true, graphView);
        }
    }

    function showGraph(recordId) {
        if (staticGraphMode) {
            switchGraphMode();
        }
        currentRecordIndex = recordId;
        getGraphData(true);
    }

    function showSelected(recordIds) {
        fetch(multiGraphUri + '/', 
        {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(recordIds)
        })
        .then(response => response.json())
        .then(data => {
            console.log(data);
            currentRecordFullData = JSON.parse(data);
            if (currentRecordFullData["executions"] == undefined) {
                return;
            }

            currentRecordDomainData = getDomainData(currentRecordFullData["executions"][currentExecutionIndex]);
            
            updateNodeGraph(true, graphView);
        });
    }
</script>

<style>
    @import "../css/site.css";
    @import "../lib/bootstrap/dist/css/bootstrap.min.css";

    div.container {
        display: flex;
        justify-content: center;
        gap: 10px;
    }
</style>

<WebRecordTable startNewExecution={startNewExecution} deleteWebSiteRecord={deleteWebSiteRecord} requestExecutionFilter={filterExecutions} showGraph={showGraph} showSelected={showSelected} bind:this={webRecordTable}></WebRecordTable>
<ExecutionsTable bind:this={executionsTable}></ExecutionsTable>

<div class="container">
    <button class="btn btn-secondary" bind:this={modeButton} on:click={switchGraphMode}>Make Static</button>
    <button class="btn btn-secondary" bind:this={viewButton} on:click={switchGraphView}>View Domains</button>
</div>

<NodeGraph bind:this={nodeGraph}></NodeGraph>