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
export let requestExecutionFilter;

let pageNumber = 0;
const numberOfItemsOnPage = 6;
let previousButton;
let nextButton;

$: WebsiteRecords = [];
$: WebsiteRecordsOnPage = [];

export function update(data) {
    WebsiteRecords = [];
    data.forEach(record => {
        let periodicity = "" + record.Days + ":" + record.Hours + ":" + record.Minutes;
        WebsiteRecords.push(new WebsiteRecord(record.Id, record.Url, record.Regex, periodicity, record.Label, record.Tags, record.LastExecutionTime, record.LastExecutionStatus));
    });
    updatePage();
    updateButtons();
}

function updatePage() {
    WebsiteRecordsOnPage = [];
    for (let i = 0; i < WebsiteRecords.length; i++) {
        if (((((pageNumber) * numberOfItemsOnPage)) <= i) && (i < ((pageNumber + 1) * numberOfItemsOnPage))) {
            WebsiteRecordsOnPage.push(WebsiteRecords[i]);
        }
    }
}

function updateButtons() {
    if ((WebsiteRecords.length <= numberOfItemsOnPage) || (((pageNumber + 1) * numberOfItemsOnPage) >= WebsiteRecords.length)) {
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
if(pageNumber < (WebsiteRecords.length / numberOfItemsOnPage)){
    pageNumber++;
    updatePage();
}
}

function previousPage() {
if(pageNumber > 0){
    pageNumber--;
    updatePage();
}
}
</script>

<div class="list">
    <h3>List of current Website Records</h3>
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
                <th>Crawl now</th>
            </tr>
        </thead>
        <tbody>
            {#each WebsiteRecordsOnPage as record}
                <tr>
                    <td contenteditable="false" bind:innerHTML={record.Url}/>
                    <td contenteditable="false" bind:innerHTML={record.Regex}/>
                    <td contenteditable="false" bind:innerHTML={record.Periodicity}/>
                    <td contenteditable="false" bind:innerHTML={record.Label}/>
                    <td contenteditable="false" bind:innerHTML={record.LastExecutionTime}/>
                    <td contenteditable="false" bind:innerHTML={record.LastExecutionStatus}/>
                    <td>
                        {#each record.Tags as tag}
                            <div contenteditable="false" bind:innerHTML={tag} />
                        {/each}
                    </td>
                    <td><button type="button" class="btn btn-primary" on:click={startNewExecution(record.Id)}>Start New Execution</button></td>
                    <td><button type="button" class="btn btn-primary" on:click={requestExecutionFilter(record.Id)}>Show Related Executions</button></td>
                </tr>
            {/each}
        </tbody>
    </table>  
</div>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={previousButton} on:click={previousPage}>Previous Page</button>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={nextButton} on:click={nextPage}>Next Page</button>