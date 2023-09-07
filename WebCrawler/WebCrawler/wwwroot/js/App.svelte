<svelte:options tag="svelte-app" />
<script>
    import NodeGraph from "./NodeGraph.svelte";
    
    const metaDataUri = '/Api/GetMetaData';
    const fullDataUri = '/Api/GetFullData';
    const formUri = '/Home/AddRecord'
    const interval = 3000;
    const executionUpdateInterval = 300;
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
    getData();
    setInterval(() => updateExecutionInformationInRecordTable(), executionUpdateInterval);
    
    console.log("here");

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
        
        return json;
    }

    function updateExecutionInformationInRecordTable() {
        console.log("test");
    }
    
</script>

<NodeGraph bind:this={graph}></NodeGraph>