import React from "react";
import {Button, Card, Flex, Form, Input, notification} from "antd";
import {LockOutlined, UserOutlined} from "@ant-design/icons";
import {doLogin} from "@/services/auth";

const Login: React.FC = () => {
    const onFinish = async (values: any) => {
        const loginApiResponse = await doLogin({
            ...values,
        });
        if (loginApiResponse.code !== "0") {
            notification.error({
                message: "登陆失败",
                description: loginApiResponse.message,
            });
        } else {
            const expireDate = new Date();
            expireDate.setTime(expireDate.getTime() + 24 * 60 * 60 * 1000);
            const loginInfo = {
                isLogin: true,
                expireDate: expireDate.toLocaleString(),
            };
            localStorage.setItem("loginInfo", JSON.stringify(loginInfo));
            window.location.href = `${process.env.contextPath}/dashboard`;
        }
    };

    return (
        <div style={{height: "100vh", backgroundColor: "rgb(248, 248, 250)"}}>
            <Flex wrap justify="center">
                <Card title="系统登陆" style={{minWidth: 350, marginTop: 100}}>
                    <Form name="loginForm" autoComplete="off" onFinish={onFinish}>
                        <Form.Item<string>
                            name="username"
                            rules={[
                                {required: true, message: "请输入用户名"},
                                {
                                    max: 20,
                                    message: "用户名不能超过20个字符",
                                },
                            ]}
                        >
                            <Input
                                prefix={<UserOutlined style={{color: "rgba(0,0,0,.25)"}}/>}
                                placeholder="用户名"
                            />
                        </Form.Item>
                        <Form.Item<string>
                            name="password"
                            rules={[{required: true, message: "请输入密码"}]}
                        >
                            <Input.Password
                                prefix={<LockOutlined style={{color: "rgba(0,0,0,.25)"}}/>}
                                placeholder="密码"
                            />
                        </Form.Item>
                        <Form.Item>
                            <Button
                                type="primary"
                                htmlType="submit"
                                style={{width: "100%"}}
                            >
                                登陆
                            </Button>
                        </Form.Item>
                    </Form>
                </Card>
            </Flex>
        </div>
    );
};

export default Login;
