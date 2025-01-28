import React, { useState } from "react";
import {
  Card,
  Row,
  Col,
  Flex,
  Button,
  Table,
  TableProps,
  Image,
  Upload,
  Space,
  Popconfirm,
  Form,
  Modal,
  Input,
  Select,
  message,
} from "antd";
import { PlusOutlined, SyncOutlined, UploadOutlined } from "@ant-design/icons";
import {
  createUser,
  doDeleteUser,
  doDisableUser,
  doEnableUser,
  fetchUserList,
  updateUser,
  User,
} from "@/services/user";
import { useRequest } from "ahooks";

const UserManagement: React.FC = () => {
  const columns: TableProps<User>["columns"] = [
    {
      title: "账号名",
      dataIndex: "userName",
      key: "userName",
    },
    {
      title: "姓名",
      dataIndex: "displayName",
      key: "displayName",
    },
    {
      title: "邮箱",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "头像",
      key: "avatarUrl",
      render: (_, record) => {
        return record.avatarUrl ? (
          <Space size="middle">
            <Image src={record.avatarUrl} width={50} height={50} />
            <Upload
              name="file"
              accept=".png,.jpg,.webp"
              action={`/api/User/UpdateUserAvatar?id=${record.id}`}
              itemRender={() => null}
              onChange={handleUploadStatusChange}
            >
              <Button icon={<UploadOutlined />}></Button>
            </Upload>
          </Space>
        ) : (
          <Upload
            name="file"
            accept=".png,.jpg,.webp"
            action={`/api/User/UpdateUserAvatar?id=${record.id}`}
            itemRender={() => null}
            onChange={handleUploadStatusChange}
          >
            <Button icon={<UploadOutlined />}></Button>
          </Upload>
        );
      },
    },
    {
      title: "角色",
      dataIndex: "roleName",
      key: "roleName",
    },
    {
      title: "状态",
      dataIndex: "statusName",
      key: "statusName",
    },
    {
      title: "创建时间",
      dataIndex: "createdAtStr",
      key: "createdAtStr",
    },
    {
      title: "操作",
      key: "action",
      render: (_, record) => {
        return (
          <Space size="middle">
            <Button
              type="primary"
              onClick={() => handleUpdateUserFormOpen(record.id, record)}
            >
              修改
            </Button>
            {record.status === 1 ? (
              <Button
                type="dashed"
                danger
                onClick={() => {
                  handleDisableUser(record.id);
                }}
              >
                禁用
              </Button>
            ) : record.status === 2 ? (
              <Button
                type="default"
                onClick={() => {
                  handleEnableUser(record.id);
                }}
              >
                启用
              </Button>
            ) : null}
            <Popconfirm
              title="请确认"
              description="确认要删除吗？"
              okText="确定"
              cancelText="取消"
              onConfirm={() => {
                handleDeleteUser(record.id);
              }}
            >
              <Button type="primary" danger>
                删除
              </Button>
            </Popconfirm>
          </Space>
        );
      },
    },
  ];

  const {
    data: userList,
    loading: userListLoading,
    runAsync: reloadUserList,
  } = useRequest(fetchUserList);

  // 查询表单
  const handleRefreshButton = async () => {
    reloadUserList(false);
  };

  //禁用和删除按钮
  const handleEnableUser = async (id: number) => {
    await doEnableUser(id);
    await reloadUserList(false);
  };
  const handleDisableUser = async (id: number) => {
    await doDisableUser(id);
    await reloadUserList(false);
  };
  const handleDeleteUser = async (id: number) => {
    await doDeleteUser(id);
    await reloadUserList(false);
  };

  // 添加成员模态表单
  const [addUserForm] = Form.useForm();
  const [addUserFormOpen, setAddUserFormOpen] = useState(false);
  const handleAddUserFormSubmit = async (values: any) => {
    await createUser({ ...values });
    setAddUserFormOpen(false);
    addUserForm.resetFields();
    await reloadUserList(false);
  };
  const handleAddUserFormOpen = () => {
    setAddUserFormOpen(true);
  };
  const handleAddUserFormOk = () => {
    addUserForm.submit();
  };
  const handleAddUserFormCancel = () => {
    setAddUserFormOpen(false);
    addUserForm.resetFields();
  };

  // 修改成员信息表单
  const [updateId, setUpdateId] = useState(0);
  const [updateUserForm] = Form.useForm();
  const [updateUserFormOpen, setUpdateUserFormOpen] = useState(false);
  const handleUpdateUserFormSubmit = async (values: any) => {
    await updateUser({ ...values, id: updateId });
    setUpdateUserFormOpen(false);
    await reloadUserList(false);
  };
  const handleUpdateUserFormOpen = (id: number, record: User) => {
    setUpdateId(id);
    updateUserForm.setFieldsValue({
      displayName: record.displayName,
      email: record.email,
      roleCode: record.roleCode,
    });
    setUpdateUserFormOpen(true);
  };
  const handleUpdateUserFormOk = async () => {
    updateUserForm.submit();
  };
  const handleUpdateUserFormCancel = async () => {
    setUpdateUserFormOpen(false);
  };

  // 修改头像
  const handleUploadStatusChange = async (info: any) => {
    if (info.file.status === "done") {
      const response = info.file.response;
      if (response?.code === "0") {
        message.success(`上传成功`);
        await reloadUserList(false);
      } else {
        message.error(response?.message ? response.message : "上传失败");
      }
    }
  };

  return (
    <>
      <Card>
        <Row gutter={[8, 8]}>
          <Col span={24}>
            <Flex justify="end" gap="small">
              <Button type="primary" onClick={handleAddUserFormOpen}>
                <PlusOutlined />
                新增
              </Button>
              <Button type="primary" onClick={handleRefreshButton}>
                <SyncOutlined />
                刷新
              </Button>
            </Flex>
          </Col>
          <Col span={24}>
            <Table
              bordered
              size="small"
              dataSource={userList}
              loading={userListLoading}
              columns={columns}
              pagination={false}
              rowKey={(record) => record.id}
            />
          </Col>
        </Row>
      </Card>
      <Modal
        title="添加成员"
        open={addUserFormOpen}
        okText="添加"
        onOk={handleAddUserFormOk}
        onCancel={handleAddUserFormCancel}
      >
        <Form
          name="addUserForm"
          form={addUserForm}
          labelCol={{ span: 4 }}
          wrapperCol={{ span: 20 }}
          autoComplete="off"
          onFinish={handleAddUserFormSubmit}
        >
          <Form.Item<string>
            name="userName"
            label="用户名"
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
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="displayName"
            label="姓名"
            rules={[
              {
                max: 20,
                message: "姓名不能超过20个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="email"
            label="邮箱"
            rules={[
              {
                max: 50,
                message: "邮箱不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="roleCode"
            label="角色"
            rules={[{ required: true, message: "请选择角色" }]}
            initialValue="MEMBER"
          >
            <Select style={{ width: 125 }}>
              <Select.Option key="member" value="MEMBER">
                用户
              </Select.Option>
              <Select.Option key="admin" value="ADMIN">
                管理员
              </Select.Option>
            </Select>
          </Form.Item>
          <Form.Item<string>
            name="password"
            label="密码"
            rules={[{ required: true, message: "请输入密码" }]}
          >
            <Input.Password />
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="更新成员信息"
        open={updateUserFormOpen}
        okText="更新"
        onOk={handleUpdateUserFormOk}
        onCancel={handleUpdateUserFormCancel}
      >
        <Form
          name="updateUserForm"
          form={updateUserForm}
          labelCol={{ span: 4 }}
          wrapperCol={{ span: 20 }}
          autoComplete="off"
          onFinish={handleUpdateUserFormSubmit}
        >
          <Form.Item<string>
            name="displayName"
            label="姓名"
            rules={[
              {
                max: 20,
                message: "姓名不能超过20个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="email"
            label="邮箱"
            rules={[
              {
                max: 50,
                message: "邮箱不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="roleCode"
            label="角色"
            rules={[{ required: true, message: "请选择角色" }]}
            initialValue="MEMBER"
          >
            <Select style={{ width: 125 }}>
              <Select.Option key="member" value="MEMBER">
                用户
              </Select.Option>
              <Select.Option key="admin" value="ADMIN">
                管理员
              </Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default UserManagement;
