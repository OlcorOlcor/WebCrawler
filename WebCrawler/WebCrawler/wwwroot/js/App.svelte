<svelte:options tag="svelte-app" />

<script>
    import NodeGraph from "./NodeGraph.svelte";
    import WebRecordTable from "./WebRecordTable.svelte";
    import ExecutionsTable from "./ExecutionsTable.svelte";

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

    form.addEventListener("submit", (event) => {
        let regex;
        try {
            console.log("checking");
            regex = new RegExp(regexInput.value);
            regexInput.setCustomValidity("");
        }
        catch(e) {
            console.log("checking failed");
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

    function getDomainData(websiteData) {
        let websiteNodes = websiteData["nodes"];
        let websiteLinks = websiteData["links"];
        let domainNodes = getDomainNodes(websiteNodes);
        let domainLinks = getDomainLinks(websiteLinks, domainNodes);

        return {"nodes": domainNodes, "links": domainLinks};
    }

    function getDomainLinks(websiteLinks, domainNodes) {
        let domainLinks = [];
        for (let i = 0; i < websiteLinks.length; i++) {
            let findDomainLink = domainLinks.find((domainLink) => {
                domainLink.source === getDomain(websiteLinks[i].source) &&
                domainLink.target === getDomain(websiteLinks[i].target)
            });

            if (findDomainLink === undefined) {
                domainLinks[domainLinks.length] = {
                    "source": getDomain(websiteLinks[i].source),
                    "target": getDomain(websiteLinks[i].target),
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

            if (nodeMatchIndex === -1) {
                domainNodes[domainNodes.length] = {
                    "id": nodeDomain, 
                    "title": "",
                    "crawl-time": websiteNodes[i]["crawl-time"],
                    "crawled-by": websiteNodes[i]["crawled-by"],
                    "group": domainNodes.length
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
            }
        }

        return domainNodes;
    }

    function getDomain(urlString) {
        const url = new URL(urlString);
        return url.hostname;
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
  
</script>

<style>
    @import '../lib/bootstrap/dist/css/bootstrap.min.css';
</style>

<WebRecordTable></WebRecordTable>

<ExecutionsTable></ExecutionsTable>

<h2>Visualisation</h2>

<button class="btn btn-secondary" bind:this={modeButton} on:click={switchGraphMode}>Make Static</button>
<button class="btn btn-secondary" bind:this={viewButton} on:click={switchGraphView}>View Domains</button>


<!-- TODO Could be only one NodeGraph with changing data for performace reasons -->
{#if websiteView}
    <NodeGraph bind:this={websiteGraph}></NodeGraph>
{:else}
    <NodeGraph bind:this={domainGraph}></NodeGraph>
{/if}