<svelte:options tag="web-record-table" />
<script>
class WebsiteRecord {
    constructor(Id, Url, Regex, Days, Hours, Minutes, Label, Tags) {
        this.Id = Id;
        this.Url = Url;
        this.Regex = Regex;
        this.Days = Days;
        this.Hours = Hours;
        this.Minutes = Minutes;
        this.Label = Label;
        this.Tags = Tags;
    }
}

const fullDataUri = "./Api/GetWebsiteRecords"
$: WebsiteRecords = [];

getWebRecords();

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
        jsonData["WebsiteRecords"].forEach(record => {
            WebsiteRecords.push(new WebsiteRecord(record.Id, record.Url, record.Regex, record.Days, record.Hours, record.Minutes, record.Label, record.Tags));
        });
        console.log(WebsiteRecords);
    })
}

</script>

<div class="list">
    <h3>List of current Website Records</h3>
    <table class="table table-striped" id="update">
        <thead>
            <tr>
                <td>Url</td>
                <td>Regex</td>
                <td>Periodicity</td>
                <td>Label</td>
                <td>Time of last executions</td>
                <td>Status of last executions</td>
                <td>Tags</td>
                <td>Crawl now</td>
            </tr>
        </thead>
        <tbody>
            {#each WebsiteRecords as record (record.Url)}
                <tr>
                    <td>{record.Url}</td>
                </tr>
            {/each}
        </tbody>
    </table>  
</div>