<svelte:options tag="svelte-app" />
<script>
    import NodeGraph from "./NodeGraph.svelte";
    
    const metaDataUri = '/Api/GetMetaData';
    const fullDataUri = '/Api/GetFullData';
    const latestExecutionUri = '/Api/GetLatestExecutions'
    const formUri = '/Home/AddRecord'
    const interval = 3000;
    const executionUpdateInterval = 10000; //10 seconds
    let currentRecordIndex = 0;
    let metaData;
    let currentRecordFullData;
    let graph;

    getData();
    setInterval(() => updateExecutionInformationInRecordTable(), executionUpdateInterval);

    function getData() {
        getMetaData().then(data => metaData = data);
        getFullData().then(data => {
            currentRecordFullData = JSON.parse(data);
            console.log(graph);
            if (graph != null && currentRecordFullData["executions"] != undefined) {
                graph.update(currentRecordFullData["executions"][0]); 
                console.log(currentRecordFullData["executions"][0]);
            }
            console.log(currentRecordFullData);
        });
        
        setTimeout(getData, interval);
    }

    function getMetaData() {
        return fetch(metaDataUri)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
    }

    function getFullData() {
        return fetch(fullDataUri + "/?recordId=" + currentRecordIndex)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
        
        
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
    
</script>

<NodeGraph bind:this={graph}></NodeGraph>