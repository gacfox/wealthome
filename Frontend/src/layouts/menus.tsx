import {
  DashboardOutlined,
  CreditCardOutlined,
  TransactionOutlined,
  TagOutlined,
  TeamOutlined,
} from "@ant-design/icons";

const menus = {
  path: "/",
  routes: [
    {
      path: "/dashboard",
      component: "@/pages/Dashboard",
      name: "仪表盘",
      icon: <DashboardOutlined />,
    },
    {
      path: "/account",
      component: "@/pages/AccountManagement",
      name: "账户管理",
      icon: <CreditCardOutlined />,
    },
    {
      path: "/transfer",
      component: "@/pages/TransferManagement",
      name: "收支管理",
      icon: <TransactionOutlined />,
    },
    {
      path: "/transferType",
      component: "@/pages/TransferTypeManagement",
      name: "收支类别管理",
      icon: <TagOutlined />,
    },
    {
      path: "/user",
      component: "@/pages/UserManagement",
      name: "成员管理",
      icon: <TeamOutlined />,
    },
  ],
};

export default menus;
