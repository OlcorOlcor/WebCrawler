<svelte:options tag="svelte-app" />
<script>
    export let name;
    export let id;
    export let test;
</script>

<main>
    <h1 id="{id}">Hello {name}!</h1>
    <p>{test}</p>
</main>

<style>
    h1 {
        font-size: 5em;
    }
</style>