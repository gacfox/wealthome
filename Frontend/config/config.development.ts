import { defineConfig } from "umi";

export default defineConfig({
  proxy: {
    "/api": {
      target: "http://127.0.0.1:5207/",
      changeOrigin: true,
      pathRewrite: { "^/api": "/api" },
    },
    "/media": {
      target: "http://127.0.0.1:5207/",
      changeOrigin: true,
      pathRewrite: { "^/media": "/media" },
    },
  },
  define: {
    "process.env.contextPath": "",
  },
});
