<svelte:options tag="svelte-app" />
<script>
    import NodeGraph from "./NodeGraph.svelte";
    
    const metaDataUri = '/Api/GetMetaData';
    const fullDataUri = '/Api/GetFullData';
    const latestExecutionUri = '/Api/GetLatestExecutions'
    const formUri = '/Home/AddRecord'
    const interval = 30000;
    const executionUpdateInterval = 3000; //30 seconds
    let currentRecordIndex = 0;
    let metaData;
    let currentRecordFullData;
    let graph;

    // setInterval(() => {
    //     getMetaData().then(data => metaData = data);
    //     getFullData().then(data => { 
    //         currentRecordFullData = data; 
    //         chart.update(currentRecordFullData); 
    //     });
    // }, interval);
    //getData();
    setInterval(() => updateExecutionInformationInRecordTable(), executionUpdateInterval);

    function getData() {
        getMetaData().then(data => metaData = data);
        getFullData().then(data => { 
            currentRecordFullData = data; 
            graph.update(currentRecordFullData); 
        });
        console.log(currentRecordFullData);
        setTimeout(getData, interval);
    }

    function getMetaData() {
        return fetch(metaDataUri)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
    }

    function getFullData() {
        let json = fetch(fullDataUri + "/" + currentRecordIndex)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
        
        
    }

    function updateExecutionInformationInRecordTable() {
        let json = fetch(latestExecutionUri + "/")
            .then(res => {
                if (res.ok) {
                    return res.json()
                } else {
                    throw new Error("Unable to fetch latest executions")
                }
            })
            .then(json => json)
            .catch(err => console.error(err));
    }
    
</script>

<NodeGraph bind:this={graph}></NodeGraph>