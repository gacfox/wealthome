import {
  createTransferType,
  deleteTransferType,
  fetchTransferTypeList,
  TransferType,
  updateTransferType,
} from "@/services/transferType";
import { PlusOutlined, SyncOutlined } from "@ant-design/icons";
import { useRequest } from "ahooks";
import {
  Button,
  Card,
  Col,
  Flex,
  Form,
  Input,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Table,
  TableProps,
} from "antd";
import React, { useState } from "react";

const TransferTypeManagement: React.FC = () => {
  const columns: TableProps<TransferType>["columns"] = [
    {
      title: "类别名称",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "收支类型",
      dataIndex: "inoutTypeName",
      key: "inoutTypeName",
    },
    {
      title: "操作",
      key: "action",
      render: (_, record) => {
        return (
          <Space size="middle">
            <Button
              type="primary"
              onClick={() =>
                handleUpdateTransferTypeFormOpen(record.id, record)
              }
            >
              修改
            </Button>
            <Popconfirm
              title="请确认"
              description="确认要删除吗？"
              okText="确定"
              cancelText="取消"
              onConfirm={() => {
                handleDeleteTransferType(record.id);
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
    data: transferTypeList,
    loading: transferTypeListLoading,
    runAsync: reloadTransferTypeList,
  } = useRequest(fetchTransferTypeList);

  // 查询表单
  const handleRefreshButton = () => {
    reloadTransferTypeList();
  };

  // 删除按钮
  const handleDeleteTransferType = async (id: number) => {
    await deleteTransferType(id);
    await reloadTransferTypeList();
  };

  // 添加交易类别表单
  const [addTransferTypeForm] = Form.useForm();
  const [addTransferTypeFormOpen, setAddTransferTypeFormOpen] = useState(false);
  const handleAddTransferTypeFormSubmit = async (values: any) => {
    await createTransferType({ ...values });
    setAddTransferTypeFormOpen(false);
    addTransferTypeForm.resetFields();
    await reloadTransferTypeList();
  };
  const handleAddTransferTypeFormOpen = () => {
    setAddTransferTypeFormOpen(true);
  };
  const handleAddTransferTypeFormOk = () => {
    addTransferTypeForm.submit();
  };
  const handleAddTransferTypeFormCancel = () => {
    setAddTransferTypeFormOpen(false);
    addTransferTypeForm.resetFields();
  };

  // 修改交易类别信息表单
  const [updateId, setUpdateId] = useState(0);
  const [updateTransferTypeForm] = Form.useForm();
  const [updateTransferTypeFormOpen, setUpdateTransferTypeFormOpen] =
    useState(false);
  const handleUpdateTransferTypeFormSubmit = async (values: any) => {
    await updateTransferType({ ...values, id: updateId });
    setUpdateTransferTypeFormOpen(false);
    await reloadTransferTypeList();
  };
  const handleUpdateTransferTypeFormOpen = (
    id: number,
    record: TransferType
  ) => {
    setUpdateId(id);
    updateTransferTypeForm.setFieldsValue({
      name: record.name,
      inoutType: record.inoutType.toString(),
    });
    setUpdateTransferTypeFormOpen(true);
  };
  const handleUpdateTransferTypeFormOk = () => {
    updateTransferTypeForm.submit();
  };
  const handleUpdateTransferTypeFormCancel = () => {
    setUpdateTransferTypeFormOpen(false);
  };

  return (
    <>
      <Card>
        <Row gutter={[8, 8]}>
          <Col span={24}>
            <Flex justify="end" gap="small">
              <Button type="primary" onClick={handleAddTransferTypeFormOpen}>
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
              dataSource={transferTypeList}
              loading={transferTypeListLoading}
              columns={columns}
              pagination={false}
              rowKey={(record) => record.id}
            />
          </Col>
        </Row>
      </Card>
      <Modal
        title="添加收支类别"
        open={addTransferTypeFormOpen}
        okText="添加"
        onOk={handleAddTransferTypeFormOk}
        onCancel={handleAddTransferTypeFormCancel}
      >
        <Form
          name="addTransferTypeForm"
          form={addTransferTypeForm}
          labelCol={{ span: 6 }}
          wrapperCol={{ span: 18 }}
          autoComplete="off"
          onFinish={handleAddTransferTypeFormSubmit}
        >
          <Form.Item<string>
            name="name"
            label="收支类别名称"
            rules={[
              { required: true, message: "请输入收支类别名称" },
              {
                max: 50,
                message: "收支类别名称不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<number>
            name="inoutType"
            label="收支类别类型"
            rules={[{ required: true, message: "请选择收支类别类型" }]}
            initialValue="1"
          >
            <Select style={{ width: 175 }}>
              <Select.Option key="type1" value="1">
                收入类型
              </Select.Option>
              <Select.Option key="type2" value="2">
                支出类型
              </Select.Option>
              <Select.Option key="type3" value="3">
                内部转账类型
              </Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="更新收支类别信息"
        open={updateTransferTypeFormOpen}
        okText="更新"
        onOk={handleUpdateTransferTypeFormOk}
        onCancel={handleUpdateTransferTypeFormCancel}
      >
        <Form
          name="updateTransferTypeForm"
          form={updateTransferTypeForm}
          labelCol={{ span: 6 }}
          wrapperCol={{ span: 18 }}
          autoComplete="off"
          onFinish={handleUpdateTransferTypeFormSubmit}
        >
          <Form.Item<string>
            name="name"
            label="收支类别名称"
            rules={[
              { required: true, message: "请输入收支类别名称" },
              {
                max: 50,
                message: "收支类别名称不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item<string>
            name="inoutType"
            label="收支类别类型"
            rules={[{ required: true, message: "请选择收支类别类型" }]}
            initialValue="1"
          >
            <Select style={{ width: 175 }}>
              <Select.Option key="type1" value="1">
                收入类型
              </Select.Option>
              <Select.Option key="type2" value="2">
                支出类型
              </Select.Option>
              <Select.Option key="type3" value="3">
                内部转账类型
              </Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default TransferTypeManagement;
