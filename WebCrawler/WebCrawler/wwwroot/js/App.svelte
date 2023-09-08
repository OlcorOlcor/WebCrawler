<svelte:options tag="svelte-app" />
<script>
    import NodeGraph from "./NodeGraph.svelte";
    
    const metaDataUri = '/Api/GetMetaData';
    const fullDataUri = '/Api/GetFullData';
    const formUri = '/Home/AddRecord'
    const interval = 3000;
    let currentRecordIndex = 2;
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

    function getData() {
        getMetaData().then(data => metaData = data);
        getFullData().then(data => { 
            currentRecordFullData = data; 
            if (graph != null) {
                graph.update(JSON.parse(currentRecordFullData).executions[0]); 
            }
        });
        try {
            console.log(JSON.parse(currentRecordFullData).executions[0]);
        }
        catch(e) {
            console.log(e);
        }
        
        setTimeout(getData, interval);
    }

    function getMetaData() {
        return fetch(metaDataUri)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
    }

    function getFullData() {
        return fetch(fullDataUri + "/" + currentRecordIndex)
            .then(response => response.json())
            .then(data => data)
            .catch(error => console.error('Unable to get items.', error));
        
    }

    
</script>

<NodeGraph bind:this={graph}></NodeGraph>