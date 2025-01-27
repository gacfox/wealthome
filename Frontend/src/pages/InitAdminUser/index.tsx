import React from "react";
import { Button, Card, Flex, Form, Input, notification } from "antd";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { initAdminUser } from "@/services/auth";

const InitAdminUser: React.FC = () => {
  const onFinish = async (values: any) => {
    const apiResponse = await initAdminUser({ ...values });
    if (apiResponse.code !== "0") {
      notification.error({
        message: "系统初始化失败",
        description: apiResponse.message,
      });
    } else {
      window.location.href = process.env.contextPath + "/login";
    }
  };

  return (
    <div style={{ height: "100vh", backgroundColor: "rgb(248, 248, 250)" }}>
      <Flex wrap justify="center">
        <Card
          title="管理员账号初始化"
          style={{ minWidth: 350, marginTop: 100 }}
        >
          <Form name="initAdminUserForm" autoComplete="off" onFinish={onFinish}>
            <Form.Item<string>
              name="userName"
              rules={[
                { required: true, message: "请输入用户名" },
                {
                  max: 20,
                  message: "用户名不能超过20个字符",
                },
                {
                  pattern: /^[a-zA-Z0-9]+$/,
                  message: "用户名只能包含字母和数字",
                },
              ]}
            >
              <Input
                prefix={<UserOutlined style={{ color: "rgba(0,0,0,.25)" }} />}
                placeholder="用户名"
              />
            </Form.Item>
            <Form.Item<string>
              name="password"
              rules={[{ required: true, message: "请输入密码" }]}
            >
              <Input.Password
                prefix={<LockOutlined style={{ color: "rgba(0,0,0,.25)" }} />}
                placeholder="密码"
              />
            </Form.Item>
            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                style={{ width: "100%" }}
              >
                初始化
              </Button>
            </Form.Item>
          </Form>
        </Card>
      </Flex>
    </div>
  );
};

export default InitAdminUser;
