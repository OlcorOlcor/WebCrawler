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

  let executions = [
    {label: "asdf", status: "ok", time: "12:00", nmbrOfSites: 12},
    {label: "df", status: "not ok", time: "14:00", nmbrOfSites: 42}
  ];

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
      console.log(json);
      JSON.parse(json);
    })
    .then(jsonData => {
      if( typeof jsonData["Executions"] !== 'undefined'){
        jsonData["Executions"].forEach(execution => {
        executions.push(new execution(execution.RecordLabel, execution.Status, execution.Time, execution.NumberOfSitesCrawled));
      })
    }
  })
}

</script>

<div class="list">
    <h3>List of current Executions</h3>
    <table class="table table-striped" id="update">
        <tr>
            <th>Record's label</th>
            <th>Execution status</th>
            <th>Time of start</th>
            <th>Number of sites crawled</th>
            <th>Show Execution</th>
        </tr>
      {#each executions as execution}
        <tr>
          <td>{execution.label}</td>
          <td>{execution.status}</td>
          <td>{execution.time}</td>
          <td>{execution.nmbrOfSites}</td>
          <td>NOT YET</td>
        </tr>
      {/each}
    </table>
</div>