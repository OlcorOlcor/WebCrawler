<svelte:options tag="execution-table" />
<script>

  class execution {
    constructor(recordId, label, status, starttime, endtime, nmbrOfSites){
      this.recordId = recordId;
      this.label = label;
      this.status = status;
      this.starttime = starttime;
      this.endtime = endtime;
      this.nmbrOfSites = nmbrOfSites;
    }
  }

  let pageNumber = 0;
  const MaxItemsOnPage = 6;
  let previousButton;
  let nextButton;
  let filteredExecutionCount = 0;
  let executions;
  
  let stopButton;
  let filterOn = false;
  let filterBy;

  $: tableExecutions = [];
  $: filteredExecutions = [];

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
      if( typeof jsonData["Executions"] !== undefined){
        executions = jsonData["Executions"];
        updatePage();
      }
      updateButtons();
    })
  }

  function updatePage(){
    filteredExecutionCount = 0;
    filteredExecutions = [];
    // Filter executions
    for(let i = 0; i < executions.length; i++){
      let exec = executions[i];

      if(!filterOn || filterBy == exec["RecordId"]){
        filteredExecutions.push(new execution(
          exec["RecordId"], 
          exec["RecordLabel"], 
          exec["Status"],
          exec["StartTime"], 
          exec["EndTime"], 
          exec["NumberOfSitesCrawled"]
        ));

        filteredExecutionCount++;
      }
    }

    // Decide what exections are visible
    tableExecutions = [];
    if (pageNumber >= filteredExecutionCount / MaxItemsOnPage) {
      pageNumber = filteredExecutionCount % MaxItemsOnPage == 0 ? (filteredExecutionCount / MaxItemsOnPage) : (filteredExecutionCount / MaxItemsOnPage) + 1;
    }

    // TODO rewrite to for loop with calculated start and end
    let i = 0;
    filteredExecutions.forEach((filteredExecution) => {
      if (executionOnVisiblePage(i)) {
        tableExecutions.push(filteredExecution)
      }
      i++; 
    });
  }

  function executionOnVisiblePage(count) {
    return (((pageNumber) * MaxItemsOnPage)) <= count && count < ((pageNumber + 1) * MaxItemsOnPage);
  }

  // will be used for changing filter -> WebSiteRecord
  export function filterExecutionsById(id) {
    filterOn = true;
    filterBy = id;
    updatePage();
    stopButton.disabled = false;
  }

  function stopFilter() {
    filterOn = false;
    filterBy = null;
    updatePage();
    stopButton.disabled = true;
  }

  function updateButtons(){
    if ((filteredExecutionCount <= MaxItemsOnPage) || (((pageNumber + 1) * MaxItemsOnPage) >= filteredExecutionCount)) {
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

  function nextPage(){
    if(pageNumber < (filteredExecutionCount / MaxItemsOnPage)){
      pageNumber++;
      updatePage();
    }
  }

  function previousPage(){
    if(pageNumber > 0){
      pageNumber--;
      updatePage();
    }
  }

</script>

<div class="list">
    <h3>List of current Executions</h3>
    <table class="table table-striped" id="update">
      <thead>
        <tr>
          <th>Record's label</th>
          <th>Execution status</th>
          <th>Start time</th>
          <th>End time</th>
          <th>Number of sites crawled</th>
          <th>Show Execution</th>
        </tr>
      </thead>

      <tbody>
        {#each tableExecutions as tableExecution}
        <tr>
          <td contenteditable="false" bind:innerHTML={tableExecution.label}/>
          <td contenteditable="false" bind:innerHTML={tableExecution.status}/>
          <td contenteditable="false" bind:innerHTML={tableExecution.starttime}/>
          <td contenteditable="false" bind:innerHTML={tableExecution.endtime}/>
          <td contenteditable="false" bind:innerHTML={tableExecution.nmbrOfSites}/>
          <td>NOT YET</td>
        </tr>
      {/each}
      </tbody>
    </table>
</div>

<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={previousButton} on:click={previousPage}>Previous Page</button>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={nextButton} on:click={nextPage}>Next Page</button>
<button class="btn btn-outline-dark btn-sm" disabled=true bind:this={stopButton} on:click={stopFilter}>Stop Filtering</button>
