import { defineConfig } from "umi";
import routes from "./routes";

export default defineConfig({
  routes: routes,
  npmClient: "npm",
  outputPath: "../wwwroot",
  define: {
  },
});
