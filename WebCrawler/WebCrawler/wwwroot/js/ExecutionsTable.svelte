<svelte:options tag="execution-table" />
<script>

  class execution {
    constructor(label, status, time, nmbrOfSites){
      this.label = label;
      this.status = status;
      this.time = time;
      this.nmbrOfSites = nmbrOfSites;
    }
  }

  $: allExecutions = [];

  let table;

  const url = "./Api/GetExecutions/";

  var fetchDataRepeatedly = setInterval( fetchData, 1000 );

  function fetchData() {
    fetch(url)
    .then(result => {
      if (result.ok) {
        return result.json()
      } else {
        throw new Error("Unable to fetch executions")
      }
    })
    .then(json => {
      let jsonData = JSON.parse(json);
      console.log(jsonData);
      if( typeof jsonData["Executions"] !== undefined){
        let executions = jsonData["Executions"];
        allExecutions = [];
        for(let i = 0; i < executions.length; i++){
          let exec = executions[i];
          allExecutions.push(new execution(exec["RecordLabel"], exec["Status"], exec["Time"], exec["NumberOfSitesCrawled"]));
        }
      }
    })
  }

</script>

<div class="list">
    <h3>List of current Executions</h3>
    <table class="table table-striped" id="update" bind:this={table}>
        <tr>
            <th>Record's label</th>
            <th>Execution status</th>
            <th>Time of start</th>
            <th>Number of sites crawled</th>
            <th>Show Execution</th>
        </tr>
      {#each allExecutions as oneExecution}
        <tr>
          <td contenteditable="false" bind:innerHTML={oneExecution.label}/>
          <td contenteditable="false" bind:innerHTML={oneExecution.status}/>
          <td contenteditable="false" bind:innerHTML={oneExecution.time}/>
          <td contenteditable="false" bind:innerHTML={oneExecution.nmbrOfSites}/>
          <td>NOT YET</td>
        </tr>
      {/each}
    </table>
</div>