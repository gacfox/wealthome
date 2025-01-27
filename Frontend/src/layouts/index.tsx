import React from "react";
import { PageContainer, ProLayout } from "@ant-design/pro-layout";
import { Link, Outlet, useLocation } from "umi";
import menus from "@/layouts/menus";
import { LogoutOutlined } from "@ant-design/icons";
import { ConfigProvider, Dropdown } from "antd";
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
  return (
    <ConfigProvider>
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
