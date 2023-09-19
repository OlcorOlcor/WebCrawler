<svelte:options tag="web-record-table" />
<script>

class WebsiteRecord {
    constructor(Id, Url, Regex, Periodicity, Label, Tags, LastExecutionTime, LastExecutionStatus) {
        this.Id = Id;
        this.Url = Url;
        this.Regex = Regex;
        this.Periodicity = Periodicity;
        this.Label = Label;
        this.Tags = Tags;
        this.LastExecutionTime = LastExecutionTime;
        this.LastExecutionStatus = LastExecutionStatus;
    }
}

export let startNewExecution;
export let deleteWebSiteRecord;
export let requestExecutionFilter;
export let showGraph;
export let showSelected;

let tagToFilterBy = null;
let pageNumber = 0;
const MaxItemsOnPage = 6;
let previousButton;
let nextButton;
let selectedRowId = 0;

$: WebsiteRecords = [];
$: WebsiteRecordsOnPage = [];
let SelectedRecords = [];

export function update(data) {
    WebsiteRecords = [];
    data.forEach(record => {
        let periodicity = "" + record.Days + "d:" + record.Hours + "h:" + record.Minutes + "m";
        WebsiteRecords.push(new WebsiteRecord(record.Id, record.Url, record.Regex, periodicity, record.Label, record.Tags, record.LastExecutionTime, record.LastExecutionStatus));
    });
    updateTable();
}

function updateTable() {
    if (pageNumber >= WebsiteRecords.length / MaxItemsOnPage && pageNumber != 0) {
      pageNumber = WebsiteRecords.length % MaxItemsOnPage == 0 ? (WebsiteRecords.length / MaxItemsOnPage) - 1 : (WebsiteRecords.length / MaxItemsOnPage);
    }

    if(sortBy === "Time"){
        WebsiteRecords.sort((a,b) => (a.LastExecutionTime > b.LastExecutionTime) ? 1 : -1);
    }
    else if(sortBy === "Url"){
        WebsiteRecords.sort((a,b) => (a.Url > b.Url) ? 1 : -1);
    }

    if(tagToFilterBy !== null) {
        WebsiteRecords = WebsiteRecords.filter(function(obj) {
            return obj.Tags.includes(tagToFilterBy);
        });
    }

    WebsiteRecordsOnPage = [];
    for (let i = 0; i < WebsiteRecords.length; i++) {
        if (webRecordOnVisiblePage(i)) {
            WebsiteRecordsOnPage.push(WebsiteRecords[i]);
        }
    }

    updateButtons();
}

function webRecordOnVisiblePage(count) {
    return (((pageNumber) * MaxItemsOnPage)) <= count && count < ((pageNumber + 1) * MaxItemsOnPage);
}

function updateButtons() {
    if ((WebsiteRecords.length <= MaxItemsOnPage) || (((pageNumber + 1) * MaxItemsOnPage) >= WebsiteRecords.length)) {
      nextButton.disabled = true;
    }
    else {
      nextButton.disabled = false;
    }
    if (pageNumber == 0) {
      previousButton.disabled = true;
    }
    else {
      previousButton.disabled = false;
    }
}
function nextPage() {
    if(pageNumber < (WebsiteRecords.length / MaxItemsOnPage)){
        pageNumber++;
        updateTable();
    }
}

function previousPage() {
    if(pageNumber > 0){
        pageNumber--;
        updateTable();
    }
}


//TODO: For some reason the getELementById method doesn't work.
function ShowGraph(recordId) {
    let oldRow = document.getElementById("row-" + selectedRowId);
    let currentRow = document.getElementById("row-" + recordId);
    console.log(oldRow);
    console.log(currentRow);
    console.log("row-" + selectedRowId);
    console.log("row-" + recordId);
    oldRow.classList.remove("selected");
    currentRow.classList.add("selected");
    selectedRowId = recordId;
    showGraph(recordId);
}

function deleteRecord(record){
    console.log(record);
    console.log(WebsiteRecords);
    console.log(WebsiteRecordsOnPage);
    if(WebsiteRecords.includes(record)){
        WebsiteRecords.splice(WebsiteRecords.indexOf(record), 1);
    }
    if(WebsiteRecordsOnPage.includes(record)){
        WebsiteRecordsOnPage.splice(WebsiteRecordsOnPage.indexOf(record), 1);
    }
}

function selectRecord(recordId) {
    if (SelectedRecords.includes(recordId)) {
        SelectedRecords = SelectedRecords.filter(id => id !== recordId);
    } else {
        SelectedRecords.push(recordId);
    }
}

let sortButton;
let sortBy = "Default";
function nextSort(){
    if(sortButton === "Sort by: Default"){
        sortButton = "Sort by: Time Crawled";
        sortBy = "Time";
    }
    else if(sortButton === "Sort by: Time Crawled"){
        sortButton = "Sort by: Url";
        sortBy = "Url";
    }
    else{
        sortButton = "Sort by: Default";
        sortBy = "Default";
    }
    updateTable();
}

function setFilterTag(tag){
    tagToFilterBy = tag;
}

function stopFiltering(){
    tagToFilterBy = null;
}

</script>

<div class="list">
    <h3>List of current Website Records</h3>
    <button class="btn btn-primary" type="button" on:click={showSelected(SelectedRecords)}>Show selected</button>
    <table class="table table-striped" id="update">
        <thead>
            <tr>
                <th>Url</th>
                <th>Regex</th>
                <th>Periodicity</th>
                <th>Label</th>
                <th>Time of last executions</th>
                <th>Status of last executions</th>
                <th>Tags</th>
                <th>Select</th>
                <th>Crawl now</th>
                <th>Filter Executions</th>
                <th>Show Graph</th>
                <th>Delete</th>
            </tr>
        </thead>
        <tbody>
            {#each WebsiteRecordsOnPage as record}
                <tr id="row-{record.Id}">
                    <td contenteditable="false" bind:innerHTML={record.Url}/>
                    <td contenteditable="false" bind:innerHTML={record.Regex}/>
                    <td contenteditable="false" bind:innerHTML={record.Periodicity}/>
                    <td contenteditable="false" bind:innerHTML={record.Label}/>
                    <td contenteditable="false" bind:innerHTML={record.LastExecutionTime}/>
                    <td contenteditable="false" bind:innerHTML={record.LastExecutionStatus}/>
                    <td>
                        {#each record.Tags as tag}
                            <button class="btn btn-primary" contenteditable="false" bind:innerHTML={tag} on:click={setFilterTag(tag)}/>
                        {/each}
                    </td>
                    <td><input type="checkbox" on:change={() => selectRecord(record.Id)} value={record.Id} name="select-{record.Id}"/></td>
                    <td><button type="button" class="btn btn-primary" on:click={startNewExecution(record.Id)}>Start New Execution</button></td>
                    <td><button type="button" class="btn btn-primary" on:click={requestExecutionFilter(record.Id)}>Show Related Executions</button></td>
                    <td><button type="button" class="btn btn-primary" on:click={showGraph(record.Id)}>Show Graph</button></td>
                    <td><button type="button" class="btn btn-danger" on:click={deleteWebSiteRecord(record.Id)}>Delete Record</button></td>
                </tr>
            {/each}
        </tbody>
    </table>  
</div>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={previousButton} on:click={previousPage}>Previous Page</button>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={nextButton} on:click={nextPage}>Next Page</button>
<button class="btn btn-outline-secondary btn-sm" contenteditable="false" bind:innerHTML={sortButton} on:click={nextSort}>Sort by: Default</button>
<button class="btn btn-outline-secondary btn-sm" on:click={stopFiltering}>Stop filtering</button>

<style>
    .selected {
        background-color: blue;
    }
</style>