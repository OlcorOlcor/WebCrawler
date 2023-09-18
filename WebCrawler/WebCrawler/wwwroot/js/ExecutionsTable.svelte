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

  

  export function update(data) {
    executions = data;
    updateTable();
  }

  function updateTable(){
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
    if (pageNumber >= filteredExecutionCount / MaxItemsOnPage && pageNumber != 0) {
      pageNumber = filteredExecutionCount % MaxItemsOnPage == 0 ? (filteredExecutionCount / MaxItemsOnPage) - 1 : (filteredExecutionCount / MaxItemsOnPage);
    }

    // TODO rewrite to for loop with calculated start and end
    let i = 0;
    filteredExecutions.forEach((filteredExecution) => {
      if (executionOnVisiblePage(i)) {
        tableExecutions.push(filteredExecution)
      }
      i++; 
    });

    updateButtons();
  }

  function executionOnVisiblePage(count) {
    return (((pageNumber) * MaxItemsOnPage)) <= count && count < ((pageNumber + 1) * MaxItemsOnPage);
  }

  // will be used for changing filter -> WebSiteRecord
  export function filterExecutionsById(id) {
    filterOn = true;
    filterBy = id;
    updateTable();
    stopButton.disabled = false;
  }

  function stopFilter() {
    filterOn = false;
    filterBy = null;
    updateTable();
    stopButton.disabled = true;
  }

  function updateButtons() {
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

  function nextPage() {
    if(pageNumber < (filteredExecutionCount / MaxItemsOnPage)){
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
        </tr>
      {/each}
      </tbody>
    </table>
</div>

<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={previousButton} on:click={previousPage}>Previous Page</button>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={nextButton} on:click={nextPage}>Next Page</button>
<button class="btn btn-outline-secondary btn-sm" disabled=true bind:this={stopButton} on:click={stopFilter}>Stop Filtering</button>
