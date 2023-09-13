<svelte:options tag="execution-table" />
<script>

  class execution {
    constructor(recordId, label, status, time, nmbrOfSites){
      this.label = label;
      this.status = status;
      this.time = time;
      this.nmbrOfSites = nmbrOfSites;
      this.recordId = recordId;
    }
  }

  let pageNumber = 0;
  const numberOfItemsOnPage = 6;
  let previousButton;
  let nextButton;
  let numberOfExecutions = 0;
  let executions;

  let filteredId = false;

  $: allExecutions = [];

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
        executions = jsonData["Executions"];
        allExecutions = [];
        numberOfExecutions = 0;
        updatePage();
      }
      updateButtons();
    })
  }

  function updatePage(){
    for(let i = 0; i < executions.length; i++){
      if(((((pageNumber) * numberOfItemsOnPage)) <= i) && (i < ((pageNumber + 1) * numberOfItemsOnPage))) {
        let exec = executions[i];
        //only if not filtered show all executions
        if(filteredId === false){
          allExecutions.push(new execution(exec["RecordId"], exec["RecordLabel"], exec["Status"], exec["Time"], exec["NumberOfSitesCrawled"]));
        }
        else{
          //if filtered show only executions with given id
          if(filteredId === exec["RecordId"]){
            allExecutions.push(new execution(exec["RecordId"], exec["RecordLabel"], exec["Status"], exec["Time"], exec["NumberOfSitesCrawled"]));
          }
        }
      }
      numberOfExecutions++;
    }
  }

  // will be used for changing filter -> WebSiteRecord
  export function filterExecutionsById(id){
    filteredId = id;
  }

  function testNextFilter(){
    if(filteredId === false){
      filteredId = 0;
    }
    else if (filteredId === 3){
      filteredId = false;
    }
    else{
      filteredId++;
    }
  }

  function updateButtons(){
    if ((numberOfExecutions <= numberOfItemsOnPage) || (((pageNumber + 1) * numberOfItemsOnPage) >= numberOfExecutions)) {
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
    if(pageNumber < (numberOfExecutions / numberOfItemsOnPage)){
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

<button disabled=true bind:this={previousButton} on:click={previousPage}>Previous Page</button>
<button disabled=true bind:this={nextButton} on:click={nextPage}>Next Page</button>
<button on:click={testNextFilter}>TestNextFilter</button>
<p>Current filter:</p>
<p contenteditable="false" bind:innerHTML={filteredId}></p>