<svelte:options tag="svelte-app" />
<script>
    import TestGraph from "./TestGraph.svelte";
    
    const metaDataUri = './GetMetaData';
    const fullDataUri = './GetFullData';
    let currentRecordIndex = 0;
    let metaData;
    let currentRecordFullData;
    let graph;

    setInterval(() => {
        getMetaData().then(data => metaData = data);
        getFullData().then(data => {currentRecordFullData = data; graph.addNode(currentRecordFullData); });
    }, 2000);

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

<TestGraph bind:this={graph}></TestGraph>