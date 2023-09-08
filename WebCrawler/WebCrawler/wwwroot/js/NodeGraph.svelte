<svelte:options tag="node-graph" />

<script>
    import { onMount } from 'svelte';

    import { scaleLinear, scaleOrdinal } from 'd3-scale';
    import * as d3 from 'd3';

    export let data = {nodes: [], links: []};

    let canvas;
    let width = 1200;
    let height = 800;
    const nodeRadius = 10;
    let offset = 0;
    let infoBox;
    let nodeInfoBoxVisible = false;

    const padding = { top: 20, right: 40, bottom: 40, left: 25 };

    $: xScale = scaleLinear()
        .domain([0, 20])
        .range([padding.left, width - padding.right]);

    $: yScale = scaleLinear()
        .domain([0, 12])
        .range([height - padding.bottom, padding.top]);

    $: xTicks = width > 180 ?
        [0, 4, 8, 12, 16, 20] :
        [0, 10, 20];

    $: yTicks = height > 180 ?
        [0, 2, 4, 6, 8, 10, 12] :
        [0, 4, 8, 12];

    $: d3yScale = scaleLinear()
        .domain([0, height])
        .range([height, 0]);

    $: links = data.links.map(d => Object.create(d));
    $: nodes = data.nodes.map(d => Object.create(d));  

    const groupColour = d3.scaleOrdinal(d3.schemeCategory10);

    let simulation, context;
    onMount(() => {
        //march();
        context = canvas.getContext('2d');
        resize()
        
        simulation = d3.forceSimulation(nodes)
            .force("link", d3.forceLink(links).id(d => d.id))
            .force("charge", d3.forceManyBody()
                .strength(-100)
            )    
            .force("center", d3.forceCenter(width / 2, height / 2))
            .on("tick", () => {
            context.clearRect(0, 0, context.canvas.width, context.canvas.height);
            //
            links.forEach(d => {
                context.beginPath();
                // context.setLineDash([8,2]);
                // context.lineDashOffset = -offset;
                context.moveTo(d.source.x, d.source.y);
                context.lineTo(d.target.x, d.target.y);
                context.globalAlpha = 0.6;
                context.strokeStyle = "#999";
                context.lineWidth = 2; //Math.sqrt(d.value);
                context.stroke();
                context.globalAlpha = 1;
            });
            
            nodes.forEach((d, i) => {
                context.beginPath();
                context.arc(d.x, d.y, nodeRadius, 0, 2*Math.PI);
                context.strokeStyle = "#fff";
                context.lineWidth = 1.5;
                context.stroke();
                context.fillStyle = groupColour(d.group);
                context.fill();
                context.font = "15px Arial";
                context.fillStyle = "#000";
                context.fillText(d.id, d.x - (nodeRadius / 2), d.y + (nodeRadius / 2));
            });
        });


        // title
        d3.select(context.canvas)
            .on("click", (event) => {
                const d = simulation.find(event.offsetX, event.offsetY, nodeRadius);
                //console.log(event.offsetX, event.offsetY);
                if (d) {
                    console.log(event.x, event.y, 'title: ', d.id, ' ', d.x, d.y);
                    showNodeInfo(d.id, d.x, d.y);
                }
            });

        d3.select(context.canvas)
            .on("mousedown", (event) => {
                if (nodeInfoBoxVisible) {
                    hideNodeInfo();
                }
            });

        d3.select(canvas)
        .call(d3.drag()
            .container(canvas)
            .subject(dragsubject)
            .on("start", dragstarted)
            .on("drag", dragged)
            .on("end", dragended));
    });

    // Use the d3-force simulation to locate the node
    function dragsubject(event) {
        return simulation.find(event.x, event.y, nodeRadius);
    }

    function resize() {
        ({ width, height } = canvas);
        console.log('resize()', width, height)
    }

    function dragstarted(event) {
        if (!event.active) simulation.alphaTarget(0.3).restart();
        event.subject.fx = event.subject.x;
        event.subject.fy = event.subject.y;
    }

    function dragged(event) {
        event.subject.fx = event.x;
        event.subject.fy = event.y;
    }

    function dragended(event) {
        if (!event.active) simulation.alphaTarget(0);
        event.subject.fx = null;
        event.subject.fy = null;
    }

    export function update(newGraphData) {
        if (newGraphData.nodes == null && newGraphData.links == null) {
            return;
        } 
        
        let newNodes = newGraphData.nodes.map(d => Object.create(d));
        let newLinks = newGraphData.links.map(d => Object.create(d));
        
        let noUpdateNeeded = true;
        
        //both inner forEaches should be replaced with more efficient hashset or something

        newNodes.forEach((newNode) => {
            let nodeAlreadyPresent = false;

            nodes.forEach((node) => {
                if (node.id === newNode.id) {
                    nodeAlreadyPresent = true;
                }
            });

            if (!nodeAlreadyPresent) {
                newNode.x = (width / 2) + d3.randomInt(-5,5);
                newNode.y = (height / 2) + d3.randomInt(-5,5);
                nodes.push(newNode);

                noUpdateNeeded = false;
            }
        });

        simulation.nodes(nodes);

        newLinks.forEach((newLink) => {
            let linkAlreadyPresent = false;
            
            links.forEach((link) => {
                //console.log(newLink.source, link.source);
                if (link.source.id === newLink.source && link.target.id === newLink.target) {
                    linkAlreadyPresent = true;
                }
            });

            if (!linkAlreadyPresent) {
                links.push(newLink);
                simulation.force("link", d3.forceLink(links).id(d => d.id));

                noUpdateNeeded = false;
            }
        });
        
        if (noUpdateNeeded) {
            return;
        }

        simulation.alpha(1);
        simulation.restart();
    }

    let i = 0;
    export function addNode(node) {
        console.log(node);
        nodes.push({id: "idk" + i, group: 2, x: width/2, y:height/2});
        links.push({source: "idk" + i, target: "Bahorel", "value": 1});
        links.push({source: "idk" + i, target: "Valjean", "value": 1});
        simulation.nodes(nodes);
        simulation.alpha(0.2);
        simulation.restart();
        i++;
    }

    function march() {
        offset += 1;
        if (offset > 16) {
            offset = 0;
        }
        setTimeout(march, 10);  
    }

    function hideNodeInfo() {
        infoBox.style.display = 'none';
        nodeInfoBoxVisible = false;
    }

    function showNodeInfo(id, x, y) {
        //console.log(id, canvas.getBoundingClientRect() ,infoBox);
        let canvasBounds = canvas.getBoundingClientRect();
        infoBox.innerText = id;
        infoBox.style.top = (canvasBounds.top + window.scrollY+ y).toString() + "px";
        infoBox.style.left = (canvasBounds.left + window.scrollX + x).toString() + "px";

        infoBox.style.display ='block';
        nodeInfoBoxVisible = true;
    }

</script>

<style>
    .nodeInfo { 
        position: fixed;
        top: 0;
        left: 0;
        border: 3px solid #73AD21;
    }
</style>

<!-- <svelte:window on:resize='{resize}'/> -->
<div class='container'>
    <canvas bind:this={canvas} width={width} height={height}/>
</div>


<div style="
            display: none; 
            position: absolute;
            border: 3px solid darkgrey; 
            border-radius: 6px;
            background-color: white;
            padding: 5px;
            "
    class="nodeInfo" bind:this={infoBox}>
</div>



