import { defineConfig, searchForWorkspaceRoot } from "vite";
import react from "@vitejs/plugin-react";
import deno from "@deno/vite-plugin";
import tailwind from "@tailwindcss/vite";
import mdx from "@mdx-js/rollup";
import { tanstackRouter } from "@tanstack/router-plugin/vite";

// https://vite.dev/config/
export default defineConfig({
  base: "https://criusnyx.github.io/sew/",
  plugins: [
    deno(),
    mdx({ providerImportSource: "@mdx-js/react" }),
    tailwind(),
    tanstackRouter({
      target: "react",
      routesDirectory: "./src/routes",
      autoCodeSplitting: true,
      addExtensions: true,
    }),
    react(),
  ],
  server: {
    fs: {
      allow: [
        searchForWorkspaceRoot(process.cwd()),
        "../jsi/bin/Release/net10.0/publish/wwwroot/",
      ],
    },
  },
});
