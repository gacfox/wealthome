import dayjs from "dayjs";
import { Account, fetchAccountList } from "@/services/account";
import {
  createTransfer,
  fetchTransferListByPage,
  QueryTransferForm,
  revertTransfer,
  Transfer,
  updateTransfer,
} from "@/services/transfer";
import { fetchTransferTypeList, TransferType } from "@/services/transferType";
import { fetchUserList, User } from "@/services/user";
import { PlusOutlined, SyncOutlined } from "@ant-design/icons";
import { useRequest } from "ahooks";
import {
  Button,
  Card,
  Col,
  Flex,
  Form,
  Input,
  Popconfirm,
  Row,
  Select,
  Space,
  Table,
  TableProps,
  DatePicker,
  Modal,
  InputNumber,
  List,
} from "antd";
import Decimal from "decimal.js";
import React, { useEffect, useState } from "react";

const TransferManagement: React.FC = () => {
  const calcTransferFlowIncome = (transfer: Transfer): string => {
    let total = new Decimal(0);
    transfer.flowDtos.forEach((flow) => {
      if (flow.accountDto.accountType === 2) {
        total = total.plus(new Decimal(flow.amountStr));
      }
    });
    return total.negated().toFixed(2);
  };

  const calcTransferFlowExpense = (transfer: Transfer): string => {
    let total = new Decimal(0);
    transfer.flowDtos.forEach((flow) => {
      if (flow.accountDto.accountType === 3) {
        total = total.plus(new Decimal(flow.amountStr));
      }
    });
    return total.toFixed(2);
  };

  const columns: TableProps<Transfer>["columns"] = [
    {
      title: "标题",
      dataIndex: "title",
      key: "title",
    },
    {
      title: "收支类别",
      key: "transferTypeDto.name",
      render: (_, record) => {
        return (
          <>
            {record.transferTypeDto?.inoutType === 1 ? (
              <span style={{ color: "green" }}>
                [{record.transferTypeDto?.inoutTypeName}]
              </span>
            ) : record.transferTypeDto?.inoutType === 2 ? (
              <span style={{ color: "red" }}>
                [{record.transferTypeDto?.inoutTypeName}]
              </span>
            ) : record.transferTypeDto?.inoutType === 3 ? (
              <span style={{ color: "gray" }}>
                [{record.transferTypeDto?.inoutTypeName}]
              </span>
            ) : null}
            &nbsp;
            {record.transferTypeDto?.name}
          </>
        );
      },
    },
    {
      title: "成员",
      dataIndex: ["userDto", "displayName"],
      key: "userDto.displayName",
    },
    {
      title: "金额",
      key: "flow",
      render: (_, record) => {
        return (
          <>
            {record.transferTypeDto?.inoutType === 1 ? (
              <span style={{ color: "green" }}>
                +{calcTransferFlowIncome(record)}
              </span>
            ) : record.transferTypeDto?.inoutType === 2 ? (
              <span style={{ color: "red" }}>
                -{calcTransferFlowExpense(record)}
              </span>
            ) : record.transferTypeDto?.inoutType === 3 ? (
              <span style={{ color: "gray" }}>-</span>
            ) : null}
          </>
        );
      },
    },
    {
      title: "交易时间",
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
              type="default"
              onClick={() => handleTransferDetailOpen(record.id, record)}
            >
              详情
            </Button>
            <Button
              type="primary"
              onClick={() => handleUpdateTransferFormOpen(record.id, record)}
            >
              修改
            </Button>
            <Popconfirm
              title="请确认"
              description="确认要撤销吗？"
              okText="确定"
              cancelText="取消"
              onConfirm={() => {
                handleRevertTransfer(record.id);
              }}
            >
              <Button type="primary" danger>
                撤销
              </Button>
            </Popconfirm>
          </Space>
        );
      },
    },
  ];

  const [searchParams, setSearchParams] = React.useState(
    {} as QueryTransferForm
  );
  const {
    data: transferPagination,
    loading: transferPaginationLoading,
    runAsync: reloadTransferPagination,
  } = useRequest(
    async () => await fetchTransferListByPage({ ...searchParams })
  );

  // 查询表单
  const {
    data: userList,
    loading: userListLoading,
    runAsync: reloadUserList,
  } = useRequest(fetchUserList);

  const {
    data: transferTypeList,
    loading: transferTypeListLoading,
    runAsync: reloadTransferTypeList,
  } = useRequest(fetchTransferTypeList);

  const {
    data: accountList,
    loading: accountListLoading,
    runAsync: reloadAccountList,
  } = useRequest(fetchAccountList);

  const handleRefreshButton = () => {
    reloadTransferPagination();
  };

  useEffect(() => {
    reloadTransferPagination();
  }, [searchParams]);

  const handleSubmitSearchTransferForm = async (values: any) => {
    const postData = {
      ...values,
      fromDate:
        values.createdAtRange && values.createdAtRange[0]?.format("YYYY-MM-DD"),
      toDate:
        values.createdAtRange && values.createdAtRange[1]?.format("YYYY-MM-DD"),
      pageSize: 20,
      pageNum: 1,
    };
    setSearchParams(postData);
  };

  const handleTableChange = async (pagination: any) => {
    setSearchParams({
      ...searchParams,
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
    });
  };

  // 撤销按钮
  const handleRevertTransfer = async (id: number) => {
    await revertTransfer(id);
    reloadTransferPagination();
  };

  // 创建收支记录表单
  const [addTransferForm] = Form.useForm();
  const [addTransferFormOpen, setAddTransferFormOpen] = useState(false);
  const handleAddTransferFormSubmit = async (values: any) => {
    await createTransfer({
      ...values,
      amountStr: new Decimal(values.amountStr).toFixed(2),
      createdAtStr: values.createdAt.format("YYYY-MM-DD HH:mm:ss"),
    });
    setAddTransferFormOpen(false);
    addTransferForm.resetFields();
    setSearchParams({ ...searchParams, pageNum: 1 });
  };
  const handleAddTransferTypeFormOpen = async () => {
    setAddTransferFormOpen(true);
  };
  const handleAddTransferFormOk = () => {
    addTransferForm.submit();
  };
  const handleAddTransferFormCancel = () => {
    setAddTransferFormOpen(false);
    addTransferForm.resetFields();
  };

  // 更新收支记录表单
  const [updateId, setUpdateId] = useState(0);
  const [updateTransferForm] = Form.useForm();
  const [updateTransferFormOpen, setUpdateTransferFormOpen] = useState(false);
  const handleUpdateTransferFormSubmit = async (values: any) => {
    await updateTransfer({
      ...values,
      id: updateId,
      createdAtStr: values.createdAt.format("YYYY-MM-DD HH:mm:ss"),
    });
    setUpdateTransferFormOpen(false);
    await reloadTransferPagination();
  };
  const handleUpdateTransferFormOpen = async (id: number, record: Transfer) => {
    setUpdateId(id);
    updateTransferForm.setFieldsValue({
      title: record.title,
      transferTypeId: record.transferTypeId,
      description: record.description,
      createdAt: dayjs(record.createdAt),
    });
    setUpdateTransferFormOpen(true);
  };
  const handleUpdateTransferFormOk = () => {
    updateTransferForm.submit();
  };
  const handleUpdateTransferFormCancel = () => {
    setUpdateTransferFormOpen(false);
  };

  // 查看收支记录详情
  const [showingTransfer, setShowingTransfer] = useState({} as Transfer);
  const [transferDetailOpen, setTransferDetailOpen] = useState(false);
  const handleTransferDetailOpen = async (id: number, record: Transfer) => {
    setShowingTransfer(record);
    setTransferDetailOpen(true);
  };
  const handleTransferDetailClose = () => {
    setTransferDetailOpen(false);
  };

  return (
    <>
      <Card>
        <Row gutter={[8, 8]}>
          <Col span={24}>
            <Form
              name="searchTransferForm"
              onFinish={handleSubmitSearchTransferForm}
            >
              <Flex justify="start" gap="middle">
                <Form.Item name="pattern" label="关键字">
                  <Input style={{ width: 200 }} />
                </Form.Item>
                <Form.Item name="transferUserId" label="操作用户">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={userListLoading}
                  >
                    {userList?.map((item: User) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.displayName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item name="transferTypeId" label="收支类别">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={transferTypeListLoading}
                  >
                    {transferTypeList?.map((item: TransferType) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item name="createdAtRange" label="收支时间">
                  <DatePicker.RangePicker />
                </Form.Item>
              </Flex>
              <Flex justify="start" gap="middle">
                <Form.Item name="incomeAccountId" label="入账账户">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={accountListLoading}
                  >
                    {accountList?.map((item: Account) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.accountName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item name="expenseAccountId" label="出账账户">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={accountListLoading}
                  >
                    {accountList?.map((item: Account) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.accountName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item name="incomeUserId" label="入账账户归属用户">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={userListLoading}
                  >
                    {userList?.map((item: User) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.displayName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
                <Form.Item name="expenseUserId" label="出账账户归属用户">
                  <Select
                    allowClear
                    style={{ width: 125 }}
                    loading={userListLoading}
                  >
                    {userList?.map((item: User) => (
                      <Select.Option key={item.id} value={item.id}>
                        {item.displayName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>
              </Flex>
              <Flex justify="start" gap="middle">
                <Form.Item>
                  <Space size="small">
                    <Button type="default" htmlType="reset">
                      重置
                    </Button>
                    <Button type="primary" htmlType="submit">
                      搜索
                    </Button>
                  </Space>
                </Form.Item>
              </Flex>
            </Form>
          </Col>
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
              dataSource={transferPagination?.list}
              loading={transferPaginationLoading}
              columns={columns}
              rowKey={(record) => record.id}
              onChange={handleTableChange}
              pagination={{
                current: transferPagination?.current,
                pageSize: transferPagination?.pageSize,
                total: transferPagination?.total,
                defaultPageSize: 20,
                defaultCurrent: 1,
                showQuickJumper: true,
                showSizeChanger: true,
                pageSizeOptions: [20, 50, 100],
                locale: {
                  items_per_page: "条/页",
                  jump_to: "跳至",
                  page: "页",
                },
              }}
            />
          </Col>
        </Row>
      </Card>
      <Modal
        title="新增收支记录"
        open={addTransferFormOpen}
        okText="添加"
        onOk={handleAddTransferFormOk}
        onCancel={handleAddTransferFormCancel}
      >
        <Form
          name="addTransferForm"
          form={addTransferForm}
          labelCol={{ span: 5 }}
          wrapperCol={{ span: 19 }}
          autoComplete="off"
          onFinish={handleAddTransferFormSubmit}
        >
          <Form.Item
            name="title"
            label="标题"
            rules={[
              { required: true, message: "请输入标题" },
              {
                max: 50,
                message: "标题不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="transferTypeId"
            label="收支类别"
            rules={[{ required: true, message: "请选择收支类别" }]}
          >
            <Select style={{ width: 175 }} loading={transferTypeListLoading}>
              {transferTypeList?.map((item: TransferType) => (
                <Select.Option key={item.id} value={item.id}>
                  {item.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            name="description"
            label="描述"
            rules={[
              {
                max: 255,
                message: "描述不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="fromAccountId"
            label="来源账户"
            rules={[{ required: true, message: "请选择来源账户" }]}
          >
            <Select style={{ width: 175 }} loading={accountListLoading}>
              {accountList?.map((item: Account) => (
                <Select.Option key={item.id} value={item.id}>
                  {item.accountName}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            name="toAccountId"
            label="目的账户"
            rules={[{ required: true, message: "请选择目的账户" }]}
          >
            <Select style={{ width: 175 }} loading={accountListLoading}>
              {accountList?.map((item: Account) => (
                <Select.Option key={item.id} value={item.id}>
                  {item.accountName}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            name="amountStr"
            label="收支金额"
            rules={[
              { required: true, message: "请输入收支金额" },
              {
                validator: (_, value) =>
                  value > 0
                    ? Promise.resolve()
                    : Promise.reject(new Error("收支金额绝对值必须大于0")),
              },
            ]}
            initialValue="0.00"
          >
            <InputNumber step="0.01" stringMode />
          </Form.Item>
          <Form.Item
            name="createdAt"
            label="时间"
            rules={[{ required: true, message: "请选择时间" }]}
            initialValue={dayjs()}
          >
            <DatePicker showTime />
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="更新收支信息"
        open={updateTransferFormOpen}
        okText="更新"
        onOk={handleUpdateTransferFormOk}
        onCancel={handleUpdateTransferFormCancel}
      >
        <Form
          name="updateTransferForm"
          form={updateTransferForm}
          labelCol={{ span: 6 }}
          wrapperCol={{ span: 18 }}
          autoComplete="off"
          onFinish={handleUpdateTransferFormSubmit}
        >
          <Form.Item
            name="title"
            label="标题"
            rules={[
              { required: true, message: "请输入标题" },
              {
                max: 50,
                message: "标题不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="transferTypeId"
            label="收支类别"
            rules={[{ required: true, message: "请选择收支类别" }]}
          >
            <Select style={{ width: 125 }} loading={transferTypeListLoading}>
              {transferTypeList?.map((item: TransferType) => (
                <Select.Option key={item.id} value={item.id}>
                  {item.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            name="description"
            label="描述"
            rules={[
              {
                max: 255,
                message: "描述不能超过50个字符",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="createdAt"
            label="时间"
            rules={[{ required: true, message: "请选择时间" }]}
            initialValue={dayjs()}
          >
            <DatePicker showTime />
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="收支记录详情"
        open={transferDetailOpen}
        onCancel={handleTransferDetailClose}
        footer={null}
      >
        <Card>
          <Flex justify="start" style={{ marginBottom: "7px" }}>
            <div style={{ width: "75px" }}>标题：</div>
            <div>{showingTransfer?.title}</div>
          </Flex>
          <Flex justify="start" style={{ marginBottom: "7px" }}>
            <div style={{ width: "75px" }}>收支类别：</div>
            <div>
              {" "}
              {showingTransfer?.transferTypeDto?.inoutType === 1 ? (
                <span style={{ color: "green" }}>
                  [{showingTransfer?.transferTypeDto?.inoutTypeName}]
                </span>
              ) : showingTransfer?.transferTypeDto?.inoutType === 2 ? (
                <span style={{ color: "red" }}>
                  [{showingTransfer?.transferTypeDto?.inoutTypeName}]
                </span>
              ) : showingTransfer?.transferTypeDto?.inoutType === 3 ? (
                <span style={{ color: "gray" }}>
                  [{showingTransfer?.transferTypeDto?.inoutTypeName}]
                </span>
              ) : null}
              &nbsp;
              {showingTransfer?.transferTypeDto?.name}
            </div>
          </Flex>
          <Flex justify="start" style={{ marginBottom: "7px" }}>
            <div style={{ width: "75px" }}>成员：</div>
            <div>{showingTransfer?.userDto?.displayName}</div>
          </Flex>
          <Flex justify="start" style={{ marginBottom: "7px" }}>
            <div style={{ width: "75px" }}>交易时间：</div>
            <div>{showingTransfer?.createdAtStr}</div>
          </Flex>
          <Flex justify="start" style={{ marginBottom: "7px" }}>
            <div style={{ width: "75px" }}>金额：</div>
            <div>
              {showingTransfer?.transferTypeDto?.inoutType === 1 ? (
                <span style={{ color: "green" }}>
                  +{calcTransferFlowIncome(showingTransfer)}
                </span>
              ) : showingTransfer?.transferTypeDto?.inoutType === 2 ? (
                <span style={{ color: "red" }}>
                  -{calcTransferFlowExpense(showingTransfer)}
                </span>
              ) : showingTransfer?.transferTypeDto?.inoutType === 3 ? (
                <span style={{ color: "gray" }}>-</span>
              ) : null}
            </div>
          </Flex>
          <Flex justify="start">
            <div style={{ width: "75px" }}>描述：</div>
            <div>{showingTransfer?.description ?? "无"}</div>
          </Flex>
        </Card>
        <div style={{ marginTop: "10px" }}>
          <List
            bordered
            dataSource={showingTransfer?.flowDtos}
            renderItem={(flow) => (
              <List.Item>
                <Flex justify="start" gap="middle" key={flow.id}>
                  <div>{flow.accountDto?.accountName}</div>
                  <div>{flow.amountStr}</div>
                </Flex>
              </List.Item>
            )}
          />
        </div>
      </Modal>
    </>
  );
};

export default TransferManagement;
