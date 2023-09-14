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
        this.LastExecutionTime = LastExecutionStatus;
        this.LastExecutionStatus = LastExecutionStatus;
    }
}

const fullDataUri = "./Api/GetWebsiteRecords"
const startNewExecutionUri = "./Api/StartNewExecution"
$: WebsiteRecords = [];

getWebRecords();
setInterval(() => getWebRecords(), 1000);

function getWebRecords() {
    fetch(fullDataUri + "/")
    .then(res => {
        if (res.ok) {
            return res.json()
        } else {
            throw new Error("Unable to fetch latest executions")
        }
    })
    .then(json => JSON.parse(json))
    .then(jsonData => {
        WebsiteRecords = [];
        jsonData["WebsiteRecords"].forEach(record => {
            let periodicity = "" + record.Days + ":" + record.Hours + ":" + record.Minutes;
            WebsiteRecords.push(new WebsiteRecord(record.Id, record.Url, record.Regex, periodicity, record.Label, record.Tags, record.LastExecutionTime, record.LastExecutionStatus));
        });
    })
}

function startNewExecution(recordId) {
    fetch(startNewExecutionUri + "/?recordId=" + recordId);
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
            {#each WebsiteRecords as record}
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
                    <td><button type="button" class="btn btn-primary" on:click={startNewExecution(record.recordId)} >Start New Execution</button></td>
                </tr>
            {/each}
        </tbody>
    </table>  
</div>