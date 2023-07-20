import App from './App.svelte';
import NodePlot from './NodePlot.svelte';
import TestGraph from './TestGraph.svelte';

const metaDataUri = './GetMetaData';
const fullDataUri = './GetFullData';
let currentRecordIndex = 0;
let metaData;
let currentRecordFullData;

setInterval(() => {
    getMetaData().then(data => metaData = data);
    getFullData().then(data => currentRecordFullData = data);
}, 1000);

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