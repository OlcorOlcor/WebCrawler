(function (d3, d3Scale) {
    'use strict';

    function _interopNamespace(e) {
        if (e && e.__esModule) return e;
        var n = Object.create(null);
        if (e) {
            Object.keys(e).forEach(function (k) {
                if (k !== 'default') {
                    var d = Object.getOwnPropertyDescriptor(e, k);
                    Object.defineProperty(n, k, d.get ? d : {
                        enumerable: true,
                        get: function () { return e[k]; }
                    });
                }
            });
        }
        n["default"] = e;
        return Object.freeze(n);
    }

    var d3__namespace = /*#__PURE__*/_interopNamespace(d3);

    function noop() { }
    function run(fn) {
        return fn();
    }
    function blank_object() {
        return Object.create(null);
    }
    function run_all(fns) {
        fns.forEach(run);
    }
    function is_function(thing) {
        return typeof thing === 'function';
    }
    function safe_not_equal(a, b) {
        return a != a ? b == b : a !== b || ((a && typeof a === 'object') || typeof a === 'function');
    }
    function is_empty(obj) {
        return Object.keys(obj).length === 0;
    }
    function append(target, node) {
        target.appendChild(node);
    }
    function insert(target, node, anchor) {
        target.insertBefore(node, anchor || null);
    }
    function detach(node) {
        if (node.parentNode) {
            node.parentNode.removeChild(node);
        }
    }
    function element(name) {
        return document.createElement(name);
    }
    function svg_element(name) {
        return document.createElementNS('http://www.w3.org/2000/svg', name);
    }
    function text(data) {
        return document.createTextNode(data);
    }
    function space() {
        return text(' ');
    }
    function listen(node, event, handler, options) {
        node.addEventListener(event, handler, options);
        return () => node.removeEventListener(event, handler, options);
    }
    function attr(node, attribute, value) {
        if (value == null)
            node.removeAttribute(attribute);
        else if (node.getAttribute(attribute) !== value)
            node.setAttribute(attribute, value);
    }
    function children(element) {
        return Array.from(element.childNodes);
    }
    function set_data(text, data) {
        data = '' + data;
        if (text.data === data)
            return;
        text.data = data;
    }
    class HtmlTag {
        constructor(is_svg = false) {
            this.is_svg = false;
            this.is_svg = is_svg;
            this.e = this.n = null;
        }
        c(html) {
            this.h(html);
        }
        m(html, target, anchor = null) {
            if (!this.e) {
                if (this.is_svg)
                    this.e = svg_element(target.nodeName);
                /** #7364  target for <template> may be provided as #document-fragment(11) */
                else
                    this.e = element((target.nodeType === 11 ? 'TEMPLATE' : target.nodeName));
                this.t = target.tagName !== 'TEMPLATE' ? target : target.content;
                this.c(html);
            }
            this.i(anchor);
        }
        h(html) {
            this.e.innerHTML = html;
            this.n = Array.from(this.e.nodeName === 'TEMPLATE' ? this.e.content.childNodes : this.e.childNodes);
        }
        i(anchor) {
            for (let i = 0; i < this.n.length; i += 1) {
                insert(this.t, this.n[i], anchor);
            }
        }
        p(html) {
            this.d();
            this.h(html);
            this.i(this.a);
        }
        d() {
            this.n.forEach(detach);
        }
    }
    function attribute_to_object(attributes) {
        const result = {};
        for (const attribute of attributes) {
            result[attribute.name] = attribute.value;
        }
        return result;
    }

    let current_component;
    function set_current_component(component) {
        current_component = component;
    }
    function get_current_component() {
        if (!current_component)
            throw new Error('Function called outside component initialization');
        return current_component;
    }
    /**
     * The `onMount` function schedules a callback to run as soon as the component has been mounted to the DOM.
     * It must be called during the component's initialisation (but doesn't need to live *inside* the component;
     * it can be called from an external module).
     *
     * `onMount` does not run inside a [server-side component](/docs#run-time-server-side-component-api).
     *
     * https://svelte.dev/docs#run-time-svelte-onmount
     */
    function onMount(fn) {
        get_current_component().$$.on_mount.push(fn);
    }

    const dirty_components = [];
    const binding_callbacks = [];
    let render_callbacks = [];
    const flush_callbacks = [];
    const resolved_promise = /* @__PURE__ */ Promise.resolve();
    let update_scheduled = false;
    function schedule_update() {
        if (!update_scheduled) {
            update_scheduled = true;
            resolved_promise.then(flush);
        }
    }
    function add_render_callback(fn) {
        render_callbacks.push(fn);
    }
    // flush() calls callbacks in this order:
    // 1. All beforeUpdate callbacks, in order: parents before children
    // 2. All bind:this callbacks, in reverse order: children before parents.
    // 3. All afterUpdate callbacks, in order: parents before children. EXCEPT
    //    for afterUpdates called during the initial onMount, which are called in
    //    reverse order: children before parents.
    // Since callbacks might update component values, which could trigger another
    // call to flush(), the following steps guard against this:
    // 1. During beforeUpdate, any updated components will be added to the
    //    dirty_components array and will cause a reentrant call to flush(). Because
    //    the flush index is kept outside the function, the reentrant call will pick
    //    up where the earlier call left off and go through all dirty components. The
    //    current_component value is saved and restored so that the reentrant call will
    //    not interfere with the "parent" flush() call.
    // 2. bind:this callbacks cannot trigger new flush() calls.
    // 3. During afterUpdate, any updated components will NOT have their afterUpdate
    //    callback called a second time; the seen_callbacks set, outside the flush()
    //    function, guarantees this behavior.
    const seen_callbacks = new Set();
    let flushidx = 0; // Do *not* move this inside the flush() function
    function flush() {
        // Do not reenter flush while dirty components are updated, as this can
        // result in an infinite loop. Instead, let the inner flush handle it.
        // Reentrancy is ok afterwards for bindings etc.
        if (flushidx !== 0) {
            return;
        }
        const saved_component = current_component;
        do {
            // first, call beforeUpdate functions
            // and update components
            try {
                while (flushidx < dirty_components.length) {
                    const component = dirty_components[flushidx];
                    flushidx++;
                    set_current_component(component);
                    update(component.$$);
                }
            }
            catch (e) {
                // reset dirty state to not end up in a deadlocked state and then rethrow
                dirty_components.length = 0;
                flushidx = 0;
                throw e;
            }
            set_current_component(null);
            dirty_components.length = 0;
            flushidx = 0;
            while (binding_callbacks.length)
                binding_callbacks.pop()();
            // then, once components are updated, call
            // afterUpdate functions. This may cause
            // subsequent updates...
            for (let i = 0; i < render_callbacks.length; i += 1) {
                const callback = render_callbacks[i];
                if (!seen_callbacks.has(callback)) {
                    // ...so guard against infinite loops
                    seen_callbacks.add(callback);
                    callback();
                }
            }
            render_callbacks.length = 0;
        } while (dirty_components.length);
        while (flush_callbacks.length) {
            flush_callbacks.pop()();
        }
        update_scheduled = false;
        seen_callbacks.clear();
        set_current_component(saved_component);
    }
    function update($$) {
        if ($$.fragment !== null) {
            $$.update();
            run_all($$.before_update);
            const dirty = $$.dirty;
            $$.dirty = [-1];
            $$.fragment && $$.fragment.p($$.ctx, dirty);
            $$.after_update.forEach(add_render_callback);
        }
    }
    /**
     * Useful for example to execute remaining `afterUpdate` callbacks before executing `destroy`.
     */
    function flush_render_callbacks(fns) {
        const filtered = [];
        const targets = [];
        render_callbacks.forEach((c) => fns.indexOf(c) === -1 ? filtered.push(c) : targets.push(c));
        targets.forEach((c) => c());
        render_callbacks = filtered;
    }
    const outroing = new Set();
    function transition_in(block, local) {
        if (block && block.i) {
            outroing.delete(block);
            block.i(local);
        }
    }
    function mount_component(component, target, anchor, customElement) {
        const { fragment, after_update } = component.$$;
        fragment && fragment.m(target, anchor);
        if (!customElement) {
            // onMount happens before the initial afterUpdate
            add_render_callback(() => {
                const new_on_destroy = component.$$.on_mount.map(run).filter(is_function);
                // if the component was destroyed immediately
                // it will update the `$$.on_destroy` reference to `null`.
                // the destructured on_destroy may still reference to the old array
                if (component.$$.on_destroy) {
                    component.$$.on_destroy.push(...new_on_destroy);
                }
                else {
                    // Edge case - component was destroyed immediately,
                    // most likely as a result of a binding initialising
                    run_all(new_on_destroy);
                }
                component.$$.on_mount = [];
            });
        }
        after_update.forEach(add_render_callback);
    }
    function destroy_component(component, detaching) {
        const $$ = component.$$;
        if ($$.fragment !== null) {
            flush_render_callbacks($$.after_update);
            run_all($$.on_destroy);
            $$.fragment && $$.fragment.d(detaching);
            // TODO null out other refs, including component.$$ (but need to
            // preserve final state?)
            $$.on_destroy = $$.fragment = null;
            $$.ctx = [];
        }
    }
    function make_dirty(component, i) {
        if (component.$$.dirty[0] === -1) {
            dirty_components.push(component);
            schedule_update();
            component.$$.dirty.fill(0);
        }
        component.$$.dirty[(i / 31) | 0] |= (1 << (i % 31));
    }
    function init(component, options, instance, create_fragment, not_equal, props, append_styles, dirty = [-1]) {
        const parent_component = current_component;
        set_current_component(component);
        const $$ = component.$$ = {
            fragment: null,
            ctx: [],
            // state
            props,
            update: noop,
            not_equal,
            bound: blank_object(),
            // lifecycle
            on_mount: [],
            on_destroy: [],
            on_disconnect: [],
            before_update: [],
            after_update: [],
            context: new Map(options.context || (parent_component ? parent_component.$$.context : [])),
            // everything else
            callbacks: blank_object(),
            dirty,
            skip_bound: false,
            root: options.target || parent_component.$$.root
        };
        append_styles && append_styles($$.root);
        let ready = false;
        $$.ctx = instance
            ? instance(component, options.props || {}, (i, ret, ...rest) => {
                const value = rest.length ? rest[0] : ret;
                if ($$.ctx && not_equal($$.ctx[i], $$.ctx[i] = value)) {
                    if (!$$.skip_bound && $$.bound[i])
                        $$.bound[i](value);
                    if (ready)
                        make_dirty(component, i);
                }
                return ret;
            })
            : [];
        $$.update();
        ready = true;
        run_all($$.before_update);
        // `false` as a special case of no DOM component
        $$.fragment = create_fragment ? create_fragment($$.ctx) : false;
        if (options.target) {
            if (options.hydrate) {
                const nodes = children(options.target);
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.l(nodes);
                nodes.forEach(detach);
            }
            else {
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                $$.fragment && $$.fragment.c();
            }
            if (options.intro)
                transition_in(component.$$.fragment);
            mount_component(component, options.target, options.anchor, options.customElement);
            flush();
        }
        set_current_component(parent_component);
    }
    let SvelteElement;
    if (typeof HTMLElement === 'function') {
        SvelteElement = class extends HTMLElement {
            constructor() {
                super();
                this.attachShadow({ mode: 'open' });
            }
            connectedCallback() {
                const { on_mount } = this.$$;
                this.$$.on_disconnect = on_mount.map(run).filter(is_function);
                // @ts-ignore todo: improve typings
                for (const key in this.$$.slotted) {
                    // @ts-ignore todo: improve typings
                    this.appendChild(this.$$.slotted[key]);
                }
            }
            attributeChangedCallback(attr, _oldValue, newValue) {
                this[attr] = newValue;
            }
            disconnectedCallback() {
                run_all(this.$$.on_disconnect);
            }
            $destroy() {
                destroy_component(this, 1);
                this.$destroy = noop;
            }
            $on(type, callback) {
                // TODO should this delegate to addEventListener?
                if (!is_function(callback)) {
                    return noop;
                }
                const callbacks = (this.$$.callbacks[type] || (this.$$.callbacks[type] = []));
                callbacks.push(callback);
                return () => {
                    const index = callbacks.indexOf(callback);
                    if (index !== -1)
                        callbacks.splice(index, 1);
                };
            }
            $set($$props) {
                if (this.$$set && !is_empty($$props)) {
                    this.$$.skip_bound = true;
                    this.$$set($$props);
                    this.$$.skip_bound = false;
                }
            }
        };
    }

    /* wwwroot/js/App.svelte generated by Svelte v3.59.2 */

    function create_fragment$2(ctx) {
    	let main;
    	let h1;
    	let t0;
    	let t1;
    	let t2;
    	let t3;
    	let p;
    	let t4;

    	return {
    		c() {
    			main = element("main");
    			h1 = element("h1");
    			t0 = text("Hello ");
    			t1 = text(/*name*/ ctx[0]);
    			t2 = text("!");
    			t3 = space();
    			p = element("p");
    			t4 = text(/*test*/ ctx[2]);
    			this.c = noop;
    			attr(h1, "id", /*id*/ ctx[1]);
    		},
    		m(target, anchor) {
    			insert(target, main, anchor);
    			append(main, h1);
    			append(h1, t0);
    			append(h1, t1);
    			append(h1, t2);
    			append(main, t3);
    			append(main, p);
    			append(p, t4);
    		},
    		p(ctx, [dirty]) {
    			if (dirty & /*name*/ 1) set_data(t1, /*name*/ ctx[0]);

    			if (dirty & /*id*/ 2) {
    				attr(h1, "id", /*id*/ ctx[1]);
    			}

    			if (dirty & /*test*/ 4) set_data(t4, /*test*/ ctx[2]);
    		},
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(main);
    		}
    	};
    }

    function instance$2($$self, $$props, $$invalidate) {
    	let { name } = $$props;
    	let { id } = $$props;
    	let { test } = $$props;

    	$$self.$$set = $$props => {
    		if ('name' in $$props) $$invalidate(0, name = $$props.name);
    		if ('id' in $$props) $$invalidate(1, id = $$props.id);
    		if ('test' in $$props) $$invalidate(2, test = $$props.test);
    	};

    	return [name, id, test];
    }

    class App extends SvelteElement {
    	constructor(options) {
    		super();
    		const style = document.createElement('style');
    		style.textContent = `h1{font-size:5em}`;
    		this.shadowRoot.appendChild(style);

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance$2,
    			create_fragment$2,
    			safe_not_equal,
    			{ name: 0, id: 1, test: 2 },
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["name", "id", "test"];
    	}

    	get name() {
    		return this.$$.ctx[0];
    	}

    	set name(name) {
    		this.$$set({ name });
    		flush();
    	}

    	get id() {
    		return this.$$.ctx[1];
    	}

    	set id(id) {
    		this.$$set({ id });
    		flush();
    	}

    	get test() {
    		return this.$$.ctx[2];
    	}

    	set test(test) {
    		this.$$set({ test });
    		flush();
    	}
    }

    customElements.define("svelte-app", App);

    /* wwwroot/js/NodePlot.svelte generated by Svelte v3.59.2 */

    function create_fragment$1(ctx) {
    	let html_tag;
    	let raw_value = /*svg*/ ctx[0].node().outerHTML + "";
    	let t0;
    	let t1_value = console.log(/*svg*/ ctx[0].node()) + "";
    	let t1;

    	return {
    		c() {
    			html_tag = new HtmlTag(false);
    			t0 = space();
    			t1 = text(t1_value);
    			this.c = noop;
    			html_tag.a = t0;
    		},
    		m(target, anchor) {
    			html_tag.m(raw_value, target, anchor);
    			insert(target, t0, anchor);
    			insert(target, t1, anchor);
    		},
    		p: noop,
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) html_tag.d();
    			if (detaching) detach(t0);
    			if (detaching) detach(t1);
    		}
    	};
    }

    function dragged(event) {
    	event.subject.fx = event.x;
    	event.subject.fy = event.y;
    }

    function instance$1($$self, $$props, $$invalidate) {
    	let { width = 928 } = $$props;
    	let { height = 600 } = $$props;

    	let { data = {
    		nodes: [
    			{ id: "Myriel", group: 1 },
    			{ id: "Random", group: 1 },
    			{ id: "NotRandom", group: 2 }
    		],
    		links: [
    			{
    				source: "Myriel",
    				target: "Random",
    				value: 1
    			},
    			{
    				source: "NotRandom",
    				target: "Random",
    				value: 2
    			}
    		]
    	} } = $$props;

    	onMount(() => {
    		return svg.node();
    	});

    	// Specify the color scale.
    	let color = d3__namespace.scaleOrdinal(d3__namespace.schemeCategory10);

    	// The force simulation mutates links and nodes, so create a copy
    	// so that re-evaluating this cell produces the same result.
    	const links = data.links.map(d => ({ ...d }));

    	const nodes = data.nodes.map(d => ({ ...d }));

    	// Create a simulation with several forces.
    	const simulation = d3__namespace.forceSimulation(nodes).force("link", d3__namespace.forceLink(links).id(d => d.id)).force("charge", d3__namespace.forceManyBody()).force("center", d3__namespace.forceCenter(width / 2, height / 2)).on("tick", ticked);

    	// Create the SVG container.
    	const svg = d3__namespace.create("svg").attr("width", width).attr("height", height).attr("viewBox", [0, 0, width, height]).attr("style", "max-width: 100%; height: auto;");

    	// Add a line for each link, and a circle for each node.
    	const link = svg.append("g").attr("stroke", "#999").attr("stroke-opacity", 0.6).selectAll().data(links).join("line").attr("stroke-width", d => Math.sqrt(d.value));

    	const node = svg.append("g").attr("stroke", "#fff").attr("stroke-width", 1.5).selectAll().data(nodes).join("circle").attr("r", 5).attr("fill", d => color(d.group));
    	node.append("title").text(d => d.id);

    	// Add a drag behavior.
    	node.call(d3__namespace.drag().on("start", dragstarted).on("drag", dragged).on("end", dragended));

    	// Set the position attributes of links and nodes each time the simulation ticks.
    	function ticked() {
    		link.attr("x1", d => d.source.x).attr("y1", d => d.source.y).attr("x2", d => d.target.x).attr("y2", d => d.target.y);
    		node.attr("cx", d => d.x).attr("cy", d => d.y);
    	}

    	// Reheat the simulation when drag starts, and fix the subject position.
    	function dragstarted(event) {
    		if (!event.active) simulation.alphaTarget(0.3).restart();
    		event.subject.fx = event.subject.x;
    		event.subject.fy = event.subject.y;
    	}

    	// Restore the target alpha so the simulation cools after dragging ends.
    	// Unfix the subject position now that it’s no longer being dragged.
    	function dragended(event) {
    		if (!event.active) simulation.alphaTarget(0);
    		event.subject.fx = null;
    		event.subject.fy = null;
    	}

    	$$self.$$set = $$props => {
    		if ('width' in $$props) $$invalidate(1, width = $$props.width);
    		if ('height' in $$props) $$invalidate(2, height = $$props.height);
    		if ('data' in $$props) $$invalidate(3, data = $$props.data);
    	};

    	return [svg, width, height, data];
    }

    class NodePlot extends SvelteElement {
    	constructor(options) {
    		super();

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance$1,
    			create_fragment$1,
    			safe_not_equal,
    			{ width: 1, height: 2, data: 3 },
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["width", "height", "data"];
    	}

    	get width() {
    		return this.$$.ctx[1];
    	}

    	set width(width) {
    		this.$$set({ width });
    		flush();
    	}

    	get height() {
    		return this.$$.ctx[2];
    	}

    	set height(height) {
    		this.$$set({ height });
    		flush();
    	}

    	get data() {
    		return this.$$.ctx[3];
    	}

    	set data(data) {
    		this.$$set({ data });
    		flush();
    	}
    }

    customElements.define("node-plot", NodePlot);

    /* wwwroot/js/TestGraph.svelte generated by Svelte v3.59.2 */

    function create_fragment(ctx) {
    	let h2;
    	let t1;
    	let t2_value = console.log("ikd") + "";
    	let t2;
    	let t3;
    	let div;
    	let canvas_1;
    	let mounted;
    	let dispose;

    	return {
    		c() {
    			h2 = element("h2");
    			h2.textContent = "d3 Force Directed Graph - canvas with d3 hitdetection";
    			t1 = space();
    			t2 = text(t2_value);
    			t3 = space();
    			div = element("div");
    			canvas_1 = element("canvas");
    			this.c = noop;
    			attr(canvas_1, "width", "600");
    			attr(canvas_1, "height", "500");
    			attr(div, "class", "container");
    		},
    		m(target, anchor) {
    			insert(target, h2, anchor);
    			insert(target, t1, anchor);
    			insert(target, t2, anchor);
    			insert(target, t3, anchor);
    			insert(target, div, anchor);
    			append(div, canvas_1);
    			/*canvas_1_binding*/ ctx[2](canvas_1);

    			if (!mounted) {
    				dispose = listen(window, "resize", resize);
    				mounted = true;
    			}
    		},
    		p: noop,
    		i: noop,
    		o: noop,
    		d(detaching) {
    			if (detaching) detach(h2);
    			if (detaching) detach(t1);
    			if (detaching) detach(t2);
    			if (detaching) detach(t3);
    			if (detaching) detach(div);
    			/*canvas_1_binding*/ ctx[2](null);
    			mounted = false;
    			dispose();
    		}
    	};
    }

    let width = 500;
    let height = 600;
    const nodeRadius = 5;

    function resize() {
    	
    } // ({ width, height } = canvas);

    function instance($$self, $$props, $$invalidate) {
    	let links;
    	let nodes;

    	let { graph = {
    		nodes: [
    			{ id: "Myriel", group: 1 },
    			{ id: "Random", group: 1 },
    			{ id: "NotRandom", group: 2 }
    		],
    		links: [
    			{
    				source: "Myriel",
    				target: "Random",
    				value: 1
    			},
    			{
    				source: "NotRandom",
    				target: "Random",
    				value: 20
    			}
    		]
    	} } = $$props;

    	let canvas;
    	const padding = { top: 20, right: 40, bottom: 40, left: 25 };
    	const groupColour = d3__namespace.scaleOrdinal(d3__namespace.schemeCategory10);
    	let simulation, context;

    	onMount(() => {
    		context = canvas.getContext('2d');

    		simulation = d3__namespace.forceSimulation(nodes).force("link", d3__namespace.forceLink(links).id(d => d.id)).force("charge", d3__namespace.forceManyBody()).force("center", d3__namespace.forceCenter(width / 2, height / 2)).on("tick", () => {
    			context.clearRect(0, 0, context.canvas.width, context.canvas.height);

    			//
    			links.forEach(d => {
    				context.beginPath();
    				context.moveTo(d.source.x, d.source.y);
    				context.lineTo(d.target.x, d.target.y);
    				context.globalAlpha = 0.6;
    				context.strokeStyle = "#999";
    				context.lineWidth = Math.sqrt(d.value);
    				context.stroke();
    				context.globalAlpha = 1;
    			});

    			nodes.forEach((d, i) => {
    				context.beginPath();
    				context.arc(d.x, d.y, nodeRadius, 0, 2 * Math.PI);
    				context.strokeStyle = "#fff";
    				context.lineWidth = 1.5;
    				context.stroke();
    				context.fillStyle = groupColour(d.group);
    				context.fill();
    			});
    		});

    		// title
    		d3__namespace.select(context.canvas).on("mousemove", () => {
    			const d = simulation.find(d3__namespace.event.offsetX, d3__namespace.event.offsetY, nodeRadius);

    			if (d) {
    				console.log(d3__namespace.event.x, d3__namespace.event.y, 'title: ', d.id, ' ', d.x, d.y);
    				if (context.canvas.title !== d.id) context.canvas.title = d.id;
    			} else {
    				console.log('cleared');
    				if (delete context.canvas.title !== undefined) delete context.canvas.title;
    			}
    		});

    		d3__namespace.select(canvas).call(d3__namespace.drag().container(canvas).subject(dragsubject).on("start", dragstarted).on("drag", dragged).on("end", dragended));
    	});

    	// Use the d3-force simulation to locate the node
    	function dragsubject() {
    		return simulation.find(d3__namespace.event.x, d3__namespace.event.y, nodeRadius);
    	}

    	function dragstarted() {
    		if (!d3__namespace.event.active) simulation.alphaTarget(0.3).restart();
    		d3__namespace.event.subject.fx = d3__namespace.event.subject.x;
    		d3__namespace.event.subject.fy = d3__namespace.event.subject.y;
    	}

    	function dragged() {
    		d3__namespace.event.subject.fx = d3__namespace.event.x;
    		d3__namespace.event.subject.fy = d3__namespace.event.y;
    	}

    	function dragended() {
    		if (!d3__namespace.event.active) simulation.alphaTarget(0);
    		d3__namespace.event.subject.fx = null;
    		d3__namespace.event.subject.fy = null;
    	}

    	function canvas_1_binding($$value) {
    		binding_callbacks[$$value ? 'unshift' : 'push'](() => {
    			canvas = $$value;
    			$$invalidate(0, canvas);
    		});
    	}

    	$$self.$$set = $$props => {
    		if ('graph' in $$props) $$invalidate(1, graph = $$props.graph);
    	};

    	$$self.$$.update = () => {
    		if ($$self.$$.dirty & /*graph*/ 2) {
    			links = graph.links.map(d => Object.create(d));
    		}

    		if ($$self.$$.dirty & /*graph*/ 2) {
    			nodes = graph.nodes.map(d => Object.create(d));
    		}
    	};

    	d3Scale.scaleLinear().domain([0, 20]).range([padding.left, width - padding.right]);
    	d3Scale.scaleLinear().domain([0, 12]).range([height - padding.bottom, padding.top]);
    	d3Scale.scaleLinear().domain([0, height]).range([height, 0]);
    	return [canvas, graph, canvas_1_binding];
    }

    class TestGraph extends SvelteElement {
    	constructor(options) {
    		super();
    		const style = document.createElement('style');
    		style.textContent = `canvas{float:left}`;
    		this.shadowRoot.appendChild(style);

    		init(
    			this,
    			{
    				target: this.shadowRoot,
    				props: attribute_to_object(this.attributes),
    				customElement: true
    			},
    			instance,
    			create_fragment,
    			safe_not_equal,
    			{ graph: 1 },
    			null
    		);

    		if (options) {
    			if (options.target) {
    				insert(options.target, this, options.anchor);
    			}

    			if (options.props) {
    				this.$set(options.props);
    				flush();
    			}
    		}
    	}

    	static get observedAttributes() {
    		return ["graph"];
    	}

    	get graph() {
    		return this.$$.ctx[1];
    	}

    	set graph(graph) {
    		this.$$set({ graph });
    		flush();
    	}
    }

    customElements.define("test-graph", TestGraph);

})(d3, d3Scale);
