<svelte:options tag="svelte-app" />
<script>
    import NodeChart from "./NodeChart.svelte";
    
    const metaDataUri = './GetMetaData';
    const fullDataUri = './GetFullData';
    const interval = 5000;
    let currentRecordIndex = 0;
    let metaData;
    let currentRecordFullData;
    let chart;
    

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
            chart.update(currentRecordFullData); 
        });
        setTimeout(getData, interval)
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

<NodeChart bind:this={chart}></NodeChart>