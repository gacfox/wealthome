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
  const handleLogout = async () => {
    localStorage.removeItem("loginInfo");
    await doLogout();
    window.location.href = `${process.env.contextPath}/login`;
  };

  // 黑白主题处理
  let initialTheme =
    window.matchMedia &&
    window.matchMedia("(prefers-color-scheme: dark)").matches
      ? "dark"
      : "default";
  const overrideTheme = localStorage.getItem("theme");
  initialTheme = overrideTheme ? overrideTheme : initialTheme;
  const [currentTheme, setCurrentTheme] = React.useState(initialTheme);
  const setDarkTheme = () => {
    localStorage.setItem("theme", "dark");
    setCurrentTheme("dark");
  };
  const setLightTheme = () => {
    localStorage.setItem("theme", "default");
    setCurrentTheme("default");
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
        logo="/logo.png"
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
              <MoonOutlined onClick={setDarkTheme} />
            ) : (
              <SunOutlined onClick={setLightTheme} />
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
