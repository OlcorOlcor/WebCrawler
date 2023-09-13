<svelte:options tag="node-graph" />

<!-- 
Modified example of the D3 Force Directed Graph example.
example: https://observablehq.com/@d3/force-directed-graph
svelte example: https://github.com/happybeing/d3-fdg-svelte 
-->

<script>
    import { onMount } from 'svelte';

    import { scaleLinear, scaleOrdinal } from 'd3-scale';
    import * as d3 from 'd3';

    export let data = {nodes: [], links: []};

    let canvas;
    let width = 1200;
    let height = 800;
    const nodeRadius = 11;
    const maxNodeTextLength = 37;
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

    let transform = d3.zoomIdentity;
    let simulation, context;
    onMount(() => {
        context = canvas.getContext('2d');
        resize()
        
        simulation = d3.forceSimulation(nodes)
            .force("link", d3.forceLink(links).id(d => d.id))
            .force("charge", d3.forceManyBody()
                .strength(-100)
            )    
            .force("center", d3.forceCenter(width / 2, height / 2, 100))
            .on("tick", simulationUpdate);


        // infoBox
        d3.select(context.canvas)
            .on("click", (event) => {
                const d = simulation.find(transform.invertX(event.offsetX), transform.invertY(event.offsetY), nodeRadius);
                //console.log(event.offsetX, event.offsetY);
                if (d) {
                    console.log(event.x, event.y, d.id, d["crawl-time"], d["crawled-by"]);
                    showNodeInfo(event.offsetX, event.offsetY, d.id, d["crawl-time"], d["crawled-by"]);
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
            .on("end", dragended))
        .call(d3.zoom()
          .scaleExtent([1 / 10, 8])
          .on('zoom', zoomed));
    });

    function simulationUpdate() {
        context.save();
        context.clearRect(0, 0, context.canvas.width, context.canvas.height);
        context.translate(transform.x, transform.y);
        context.scale(transform.k, transform.k);
        
        links.forEach(d => {
            context.beginPath();
            drawArrow(context, d.source.x, d.source.y, d.target.x, d.target.y, nodeRadius);
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
            if (d["match"] == "true") {
                context.fillStyle = groupColour(d.group);
            }
            else {
                context.fillStyle = "#dadada";
            }
            context.fill();

            context.font = "15px Arial";
            context.fillStyle = "#000";
            let text = d.title != "" ? d.title : d.id;
            if (text.length > maxNodeTextLength) {
                text = text.substring(0, 37) + "...";
            }
            context.fillText(text, d.x - (nodeRadius / 2), d.y + (nodeRadius / 2));
        });

        context.restore();
    }

    function drawArrow(context, fromX, fromY, toX, toY, nodeRadius) {
        var headlen = nodeRadius * 0.70; // length of head in pixels
        var dx = toX - fromX;
        var dy = toY - fromY;
        var angle = Math.atan2(dy, dx);
        
        // Calculate the adjusted end point
        var adjustedToX = toX - nodeRadius * Math.cos(angle);
        var adjustedToY = toY - nodeRadius * Math.sin(angle);

        context.moveTo(fromX, fromY);
        context.lineTo(adjustedToX, adjustedToY);
        context.lineTo(adjustedToX - headlen * Math.cos(angle - Math.PI / 7), adjustedToY - headlen * Math.sin(angle - Math.PI / 7));
        context.moveTo(adjustedToX, adjustedToY);
        context.lineTo(adjustedToX - headlen * Math.cos(angle + Math.PI / 7), adjustedToY - headlen * Math.sin(angle + Math.PI / 7));
    }

    function zoomed(event) {
        transform = event.transform;
        simulationUpdate();
    }

    function resize() {
        ({ width, height } = canvas);
        console.log('resize()', width, height)
    }

    // Use the d3-force simulation to locate the node
    function dragsubject(event) {
        const node = simulation.find(transform.invertX(event.x), transform.invertY(event.y), nodeRadius);
        if (node) {
            node.x = transform.applyX(node.x);
            node.y = transform.applyY(node.y);
        }
        return node;
    }

    function dragstarted(event) {
        if (!event.active) simulation.alphaTarget(0.3).restart();
        event.subject.fx = transform.invertX(event.subject.x);
        event.subject.fy = transform.invertY(event.subject.y);
    }

    function dragged(event) {
        event.subject.fx = transform.invertX(event.x);
        event.subject.fy = transform.invertY(event.y);
    }

    function dragended(event) {
        if (!event.active) simulation.alphaTarget(0);
        event.subject.fx = null;
        event.subject.fy = null;
    }

    export function updateData(newGraphData) {
        if (newGraphData.nodes == null && newGraphData.links == null) {
            return;
        } 
        
        let newNodes = newGraphData.nodes.map(d => Object.create(d));
        let newLinks = newGraphData.links.map(d => Object.create(d));
        
        let noUpdateNeeded = true;
        
        // TODO Both inner forEaches should be replaced with more efficient hashset or something

        newNodes.forEach((newNode) => {
            let nodeAlreadyPresent = false;

            nodes.forEach((node) => {
                if (node.id === newNode.id) {
                    node.title = newNode.title;
                    node["crawl-time"] = newNode["crawl-time"];
                    node["crawled-by"] = newNode["crawled-by"];
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

    function hideNodeInfo() {
        infoBox.style.display = 'none';
        nodeInfoBoxVisible = false;
    }

    function showNodeInfo(x, y, url, crawlTime, crawledBy) {
        // Delete old data
        while (infoBox.firstChild) {
            infoBox.removeChild(infoBox.lastChild);
        }

        const urlDiv = document.createElement("div");
        const urlTextNode = document.createTextNode("Url: " + url);
        urlDiv.appendChild(urlTextNode);

        infoBox.appendChild(urlDiv);
        
        if (crawlTime != "") {
            const crawlTimeDiv = document.createElement("div");
            const crawlTimeTextNode = document.createTextNode("Crawl time: " + crawlTime);
            crawlTimeDiv.appendChild(crawlTimeTextNode);

            infoBox.appendChild(crawlTimeDiv);
        }
    

        if (crawledBy.length > 0) {
            const crawledByDiv = document.createElement("div");
            const crawledByUList = document.createElement("ul");
            const crawledByUListTextNode = document.createTextNode("Crawled by: ") 
            crawledByDiv.appendChild(crawledByUListTextNode);
            for (let i = 0; i < crawledBy.length; i++) {
                const crawledByItem = document.createElement("li");
                const crawledByTextNode = document.createTextNode(crawledBy[i]);
                crawledByItem.appendChild(crawledByTextNode);
                crawledByUList.appendChild(crawledByItem);
            }
            crawledByDiv.appendChild(crawledByUList);
            crawledByUList.style.cssText = "padding-left:15px;margin-top:0px;margin-bottom:0px";

            infoBox.appendChild(crawledByDiv);
        }
        
        let canvasBounds = canvas.getBoundingClientRect();
        infoBox.style.display ='block';
        nodeInfoBoxVisible = true;
        infoBox.style.top = (canvasBounds.top + window.scrollY + y - infoBox.offsetHeight).toString() + "px";
        infoBox.style.left = (canvasBounds.left + window.scrollX + x).toString() + "px";

        
    }

</script>

<!-- <svelte:window on:resize='{resize}'/> -->
<div class='container'>
    <canvas bind:this={canvas} width={width} height={height}/>
</div>

<div style='
    display: none; 
    position: absolute;
    border: 3px solid #258cfb; 
    border-radius: 6px;
    background-color: white;
    padding: 5px;
'
class="nodeInfo" bind:this={infoBox}>
</div>



