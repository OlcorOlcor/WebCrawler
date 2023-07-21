<svelte:options tag="svelte-app" />
<script>
    import NodeChart from "./NodeChart.svelte";
    
    const metaDataUri = './GetMetaData';
    const fullDataUri = './GetFullData';
    let currentRecordIndex = 0;
    let metaData;
    let currentRecordFullData;
    let chart;

    setInterval(() => {
        getMetaData().then(data => metaData = data);
        getFullData().then(data => { 
            currentRecordFullData = data; 
            chart.addNode(currentRecordFullData); 
        });
    }, 500);

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

<NodeChart bind:this={chart}></NodeChart>