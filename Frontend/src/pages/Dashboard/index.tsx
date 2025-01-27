import {
  fetchMonthStatList,
  fetchSumStat,
  fetchUserExpenseSumStatList,
  fetchUserIncomeSumStatList,
  fetchUserMonthStatList,
  QuerySumStatForm,
} from "@/services/stat";
import { useRequest } from "ahooks";
import {
  Button,
  Card,
  Col,
  DatePicker,
  Flex,
  Form,
  Row,
  Select,
  Space,
} from "antd";
import React, { useEffect, useState } from "react";
import UserIncomeSumStatChart from "@/components/charts/UserIncomeSumStatChart";
import UserExpenseSumStatChart from "@/components/charts/UserExpenseSumStatChart";
import MonthStatChart from "@/components/charts/MonthStatChart";
import UserMonthStatChart from "@/components/charts/UserMonthStatChart";

const Dashboard: React.FC = () => {
  // 收支汇总统计
  const {
    data: sumStat,
    loading: sumStatLoading,
    runAsync: reloadSumStat,
  } = useRequest(async () => {
    return await fetchSumStat({ ...sumStatSearchParams });
  });

  const {
    data: userIncomeSumStatList,
    loading: userIncomeSumStatListLoading,
    runAsync: reloadUserIncomeSumStatList,
  } = useRequest(async () => {
    return await fetchUserIncomeSumStatList({ ...sumStatSearchParams });
  });

  const {
    data: userExpenseSumStatList,
    loading: userExpenseSumStatListLoading,
    runAsync: reloadUserExpenseSumStatList,
  } = useRequest(async () => {
    return await fetchUserExpenseSumStatList({ ...sumStatSearchParams });
  });

  const [sumStatSearchParams, setSumStatSearchParams] = useState({
    mode: "total",
    year: null,
  } as QuerySumStatForm);

  useEffect(() => {
    reloadSumStat();
    reloadUserIncomeSumStatList();
    reloadUserExpenseSumStatList();
  }, [sumStatSearchParams]);

  const handleSumStatSearchFormSubmit = (values: any) => {
    setSumStatSearchParams({
      mode: values.year ? "year" : "total",
      year: values.year?.format("YYYY") ?? null,
    });
  };

  // 收支月度统计
  const [monthStatSearchParams, setMonthStatSearchParams] = useState({
    startMonth: null,
    endMonth: null,
  });

  const {
    data: userMonthStatList,
    loading: userMonthStatListLoading,
    runAsync: reloadUserMonthStatList,
  } = useRequest(async () => {
    return await fetchUserMonthStatList({ ...monthStatSearchParams });
  });

  const {
    data: monthStatList,
    loading: monthStatListLoading,
    runAsync: reloadMonthStatList,
  } = useRequest(async () => {
    return await fetchMonthStatList({ ...monthStatSearchParams });
  });

  useEffect(() => {
    reloadUserMonthStatList();
    reloadMonthStatList();
  }, [monthStatSearchParams]);

  const handleMonthStatSearchFormSubmit = (values: any) => {
    const startMonth =
      values?.monthRange && values?.monthRange[0]
        ? values?.monthRange[0]?.format("YYYY-MM")
        : null;
    const endMonth =
      values?.monthRange && values?.monthRange[1]
        ? values?.monthRange[1]?.format("YYYY-MM")
        : null;
    setMonthStatSearchParams({
      startMonth,
      endMonth,
    });
  };

  return (
    <>
      <Row gutter={[8, 8]}>
        <Col span={24}>
          <Card>
            <Row gutter={[8, 8]}>
              <Col span={24}>
                <Form
                  name="sumStatSearchForm"
                  autoComplete="off"
                  layout="inline"
                  onFinish={handleSumStatSearchFormSubmit}
                >
                  <Form.Item name="year">
                    <DatePicker picker="year" />
                  </Form.Item>
                  <Form.Item>
                    <Button type="primary" htmlType="submit">
                      查询
                    </Button>
                  </Form.Item>
                </Form>
              </Col>
              <Col span={24}>
                <Row gutter={[8, 8]}>
                  <Col span={8}>
                    <Card loading={sumStatLoading} style={{ height: "248px" }}>
                      <div
                        style={{
                          marginBottom: "12px",
                          fontSize: "16px",
                          fontWeight: "bold",
                        }}
                      >
                        收支统计摘要
                      </div>
                      <Flex
                        justify="start"
                        gap="middle"
                        style={{ marginBottom: "8px" }}
                      >
                        <div style={{ width: "125px" }}>我的总收入：</div>
                        <div style={{ color: "green" }}>
                          +{sumStat?.myIncomeStr}
                        </div>
                      </Flex>
                      <Flex
                        justify="start"
                        gap="middle"
                        style={{ marginBottom: "8px" }}
                      >
                        <div style={{ width: "125px" }}>我的总支出：</div>
                        <div style={{ color: "red" }}>
                          -{sumStat?.myExpenseStr}
                        </div>
                      </Flex>
                      <Flex
                        justify="start"
                        gap="middle"
                        style={{ marginBottom: "8px" }}
                      >
                        <div style={{ width: "125px" }}>家庭总收入：</div>
                        <div style={{ color: "green" }}>
                          +{sumStat?.homeIncomeStr}
                        </div>
                      </Flex>
                      <Flex justify="start" gap="middle">
                        <div style={{ width: "125px" }}>家庭总支出：</div>
                        <div style={{ color: "red" }}>
                          -{sumStat?.homeExpenseStr}
                        </div>
                      </Flex>
                    </Card>
                  </Col>
                  <Col span={16}>
                    <Card>
                      <Row gutter={[8, 8]}>
                        <Col span={12}>
                          <UserIncomeSumStatChart
                            userIncomeSumStatList={userIncomeSumStatList}
                          />
                        </Col>
                        <Col span={12}>
                          <UserExpenseSumStatChart
                            userExpenseSumStatList={userExpenseSumStatList}
                          />
                        </Col>
                      </Row>
                    </Card>
                  </Col>
                </Row>
              </Col>
            </Row>
          </Card>
        </Col>
        <Col span={24}>
          <Card>
            <Row gutter={[8, 8]}>
              <Col span={24}>
                <Form
                  name="monthStatSearchForm"
                  autoComplete="off"
                  layout="inline"
                  onFinish={handleMonthStatSearchFormSubmit}
                >
                  <Form.Item name="monthRange">
                    <DatePicker.RangePicker picker="month" />
                  </Form.Item>
                  <Form.Item>
                    <Button type="primary" htmlType="submit">
                      查询
                    </Button>
                  </Form.Item>
                </Form>
              </Col>
              <Col span={24}>
                <Card>
                  <Row gutter={[8, 8]}>
                    <Col span={12}>
                      <MonthStatChart monthStatList={monthStatList} />
                    </Col>
                    <Col span={12}>
                      <UserMonthStatChart
                        userMonthStatList={userMonthStatList}
                      />
                    </Col>
                  </Row>
                </Card>
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>
    </>
  );
};

export default Dashboard;
