import svelte from 'rollup-plugin-svelte';
import resolve from '@rollup/plugin-node-resolve';
// import autoPreprocess from 'svelte-preprocess';
// import ts from '@rollup/plugin-typescript';
// import typescript from 'typescript';

export default {
    // This `main.js` file we wrote
    input: 'wwwroot/js/site.js',
    output: {
        // The destination for our bundled JavaScript
        file: 'wwwroot/js/build/bundle.js',
        // Our bundle will be an Immediately-Invoked Function Expression
        format: 'iife',
        // The IIFE return value will be assigned into a variable called `app`
        name: 'app',
    },
    plugins: [
        svelte({
            // Tell the svelte plugin where our svelte files are located
            include: 'wwwroot/**/*.svelte',
            emitCss: false,
            compilerOptions: {
                customElement: true
            },
        }),
        // ts({
        //     typescript
        // }),
        // Tell any third-party plugins that we're building for the browser
        resolve({ browser: true }),

    ]
};