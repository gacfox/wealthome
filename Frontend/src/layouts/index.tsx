import React from "react";
import { PageContainer, ProLayout } from "@ant-design/pro-layout";
import { Link, Outlet, useLocation } from "umi";
import menus from "@/layouts/menus";
import {
  GithubFilled,
  LogoutOutlined,
  MoonOutlined,
  SunOutlined,
} from "@ant-design/icons";
import { ConfigProvider, Dropdown, theme } from "antd";
import { useRequest } from "ahooks";
import { fetchLoginInfo } from "@/services/auth";
import { doLogout } from "@/services/auth";

const AdminLayout: React.FC = () => {
  const location = useLocation();
  const { data } = useRequest(fetchLoginInfo);
  const [currentTheme, setCurrentTheme] = React.useState("default");
  const handleLogout = async () => {
    localStorage.removeItem("loginInfo");
    await doLogout();
    window.location.href = `${process.env.contextPath}/login`;
  };
  return (
    <ConfigProvider
      theme={{
        algorithm:
          currentTheme === "default"
            ? theme.defaultAlgorithm
            : theme.darkAlgorithm,
      }}
    >
      <ProLayout
        title="Wealthome 家庭账本"
        layout="mix"
        route={menus}
        location={location}
        menuItemRender={(item, dom) => {
          return <Link to={item.path ?? "/"}>{dom}</Link>;
        }}
        avatarProps={{
          src: !!data?.avatarUrl ? data?.avatarUrl : "/default_avatar.png",
          size: "small",
          title: data?.displayName,
          render: (props, dom) => {
            return (
              <Dropdown
                menu={{
                  items: [
                    {
                      key: "logout",
                      icon: <LogoutOutlined />,
                      label: "退出登录",
                      onClick: handleLogout,
                    },
                  ],
                }}
              >
                {dom}
              </Dropdown>
            );
          },
        }}
        actionsRender={(props) => {
          return [
            currentTheme === "default" ? (
              <MoonOutlined onClick={() => setCurrentTheme("dark")} />
            ) : (
              <SunOutlined onClick={() => setCurrentTheme("default")} />
            ),
            <GithubFilled
              onClick={() => window.open("https://github.com/gacfox/wealthome")}
            />,
          ];
        }}
        breadcrumbRender={(routers = []) => [
          {
            path: "/",
            breadcrumbName: "首页",
          },
          ...routers,
        ]}
      >
        <PageContainer>
          <Outlet />
        </PageContainer>
      </ProLayout>
    </ConfigProvider>
  );
};

export default AdminLayout;
