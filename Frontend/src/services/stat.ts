import request from "@/utils/request";

export type SumStat = {
  myIncome: number;
  myExpense: number;
  homeIncome: number;
  homeExpense: number;
  myIncomeStr: string;
  myExpenseStr: string;
  homeIncomeStr: string;
  homeExpenseStr: string;
};

export type UserIncomeSumStat = {
  displayName: string;
  incomeStr: string;
};
export type UserExpenseSumStat = {
  displayName: string;
  expenseStr: string;
};

export type QuerySumStatForm = {
  mode: string;
  year: string | null;
};

export type MonthStat = {
  monthStr: string;
  incomeStr: string;
  expenseStr: string;
};

export type QueryMonthStatForm = {
  startMonth: string | null;
  endMonth: string | null;
};

export const fetchSumStat = async (params: QuerySumStatForm) => {
  return await request("/api/Stat/QuerySumStat", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const fetchUserIncomeSumStatList = async (params: QuerySumStatForm) => {
  return await request("/api/Stat/QueryUserIncomeSumStat", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const fetchUserExpenseSumStatList = async (params: QuerySumStatForm) => {
  return await request("/api/Stat/QueryUserExpenseSumStat", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const fetchUserMonthStatList = async (params: QueryMonthStatForm) => {
  return await request("/api/Stat/QueryUserMonthStat", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const fetchMonthStatList = async (params: QueryMonthStatForm) => {
  return await request("/api/Stat/QueryMonthStat", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};
