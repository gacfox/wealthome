export default [
  { path: "/login", component: "@/pages/Login", layout: false },
  { path: "/initAdminUser", component: "@/pages/InitAdminUser", layout: false },
  { path: "/", component: "@/pages/Dashboard", wrappers: ["@/wrappers/auth"] },
  {
    path: "/dashboard",
    component: "@/pages/Dashboard",
    wrappers: ["@/wrappers/auth"],
  },
  {
    path: "/account",
    component: "@/pages/AccountManagement",
    wrappers: ["@/wrappers/auth"],
  },
  {
    path: "/transfer",
    component: "@/pages/TransferManagement",
    wrappers: ["@/wrappers/auth"],
  },
  {
    path: "/transferType",
    component: "@/pages/TransferTypeManagement",
    wrappers: ["@/wrappers/auth"],
  },
  {
    path: "/user",
    component: "@/pages/UserManagement",
    wrappers: ["@/wrappers/auth"],
  },
];
