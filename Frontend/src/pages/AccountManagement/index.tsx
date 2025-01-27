import {
  Account,
  createAccount,
  deleteAccount,
  fetchAccountList,
  updateAccount,
} from "@/services/account";
import { PlusOutlined, SyncOutlined } from "@ant-design/icons";
import { useRequest } from "ahooks";
import {
  Button,
  Card,
  Col,
  Flex,
  Form,
  Input,
  InputNumber,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Table,
  TableProps,
} from "antd";
import React, { useState } from "react";

const AccountManagement: React.FC = () => {
  const columns: TableProps<Account>["columns"] = [
    {
      title: "账户名",
      dataIndex: "accountName",
      key: "accountName",
    },
    {
      title: "账户类别",
      dataIndex: "accountTypeName",
      key: "accountTypeName",
    },
    {
      title: "余额",
      key: "balanceStr",
      render: (_, record) => {
        return <>{record.accountType === 1 ? record.balanceStr : "-"}</>;
      },
    },
    {
      title: "归属用户",
      dataIndex: ["userDto", "displayName"],
      key: "userDto.displayName",
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
              onClick={() => handleUpdateAccountFormOpen(record.id, record)}
            >
              修改
            </Button>
            <Popconfirm
              title="请确认"
              description="确认要删除吗？"
              okText="确定"
              cancelText="取消"
              onConfirm={() => {
                handleDeleteAccount(record.id);
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
    data: accountList,
    loading: accountListLoading,
    runAsync: reloadAccountList,
  } = useRequest(fetchAccountList);

  // 查询表单
  const handleRefreshButton = () => {
    reloadAccountList();
  };

  // 删除按钮
  const handleDeleteAccount = async (id: number) => {
    await deleteAccount(id);
    await reloadAccountList();
  };

  // 添加账户表单
  const [addAccountForm] = Form.useForm();
  const [addAccountFormOpen, setAddAccountFormOpen] = useState(false);
  const handleAddAccountFormSubmit = async (values: any) => {
    await createAccount({ ...values });
    setAddAccountFormOpen(false);
    addAccountForm.resetFields();
    await reloadAccountList();
  };
  const handleAddAccountFormOpen = () => {
    setAddAccountFormOpen(true);
  };
  const handleAddAccountFormOk = () => {
    addAccountForm.submit();
  };
  const handleAddAccountFormCancel = () => {
    setAddAccountFormOpen(false);
    addAccountForm.resetFields();
  };

  // 修改账号信息表单
  const [updateId, setUpdateId] = useState(0);
  const [updateAccountForm] = Form.useForm();
  const [updateAccountFormOpen, setUpdateAccountFormOpen] = useState(false);
  const handleUpdateAccountFormSubmit = async (values: any) => {
    await updateAccount({ ...values, id: updateId });
    setUpdateAccountFormOpen(false);
    await reloadAccountList();
  };
  const handleUpdateAccountFormOpen = (id: number, record: Account) => {
    setUpdateId(id);
    updateAccountForm.setFieldsValue({
      accountName: record.accountName,
      accountType: record.accountType.toString(),
    });
    setUpdateAccountFormOpen(true);
  };
  const handleUpdateAccountFormOk = () => {
    updateAccountForm.submit();
  };
  const handleUpdateAccountFormCancel = () => {
    setUpdateAccountFormOpen(false);
  };

  return (
    <>
      <Card>
        <Row gutter={[8, 8]}>
          <Col span={24}>
            <Flex justify="end" gap="small">
              <Button type="primary" onClick={handleAddAccountFormOpen}>
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
              dataSource={accountList}
              loading={accountListLoading}
              columns={columns}
              pagination={false}
              rowKey={(record) => record.id}
            />
          </Col>
        </Row>
      </Card>
      <Modal
        title="添加账户"
        open={addAccountFormOpen}
        okText="添加"
        onOk={handleAddAccountFormOk}
        onCancel={handleAddAccountFormCancel}
      >
        <Form
          name="addAccountForm"
          form={addAccountForm}
          labelCol={{ span: 4 }}
          wrapperCol={{ span: 20 }}
          autoComplete="off"
          onFinish={handleAddAccountFormSubmit}
        >
          <Form.Item<string>
            name="accountName"
            label="账户名称"
            rules={[
              { required: true, message: "请输入账户名称" },
              {
                max: 50,
                message: "账户名称不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<number>
            name="accountType"
            label="账户类型"
            rules={[{ required: true, message: "请选择账户类型" }]}
            initialValue="1"
          >
            <Select style={{ width: 175 }}>
              <Select.Option key="type1" value="1">
                储蓄账户
              </Select.Option>
              <Select.Option key="type2" value="2">
                收入来源虚拟账户
              </Select.Option>
              <Select.Option key="type3" value="3">
                支出虚拟账户
              </Select.Option>
            </Select>
          </Form.Item>
          <Form.Item<string>
            name="balance"
            label="余额"
            rules={[{ required: true, message: "请输入余额" }]}
            initialValue="0.00"
          >
            <InputNumber<string> step="0.01" stringMode />
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="更新账户信息"
        open={updateAccountFormOpen}
        okText="更新"
        onOk={handleUpdateAccountFormOk}
        onCancel={handleUpdateAccountFormCancel}
      >
        <Form
          name="updateAccountForm"
          form={updateAccountForm}
          labelCol={{ span: 4 }}
          wrapperCol={{ span: 20 }}
          autoComplete="off"
          onFinish={handleUpdateAccountFormSubmit}
        >
          <Form.Item<string>
            name="accountName"
            label="账户名称"
            rules={[
              { required: true, message: "请输入账户名称" },
              {
                max: 50,
                message: "账户名称不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="accountType"
            label="账户类型"
            rules={[{ required: true, message: "请选择账户类型" }]}
            initialValue="1"
          >
            <Select style={{ width: 175 }}>
              <Select.Option key="type1" value="1">
                储蓄账户
              </Select.Option>
              <Select.Option key="type2" value="2">
                收入来源虚拟账户
              </Select.Option>
              <Select.Option key="type3" value="3">
                支出虚拟账户
              </Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default AccountManagement;
