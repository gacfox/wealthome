import { User } from "@/services/user";
import request from "@/utils/request";

export type Account = {
  id: number;
  accountName: string;
  accountType: number;
  accountTypeName: string;
  balanceStr: string;
  userId: number;
  userDto: User;
  createdAtStr: string;
};

export type CreateAccountForm = {
  accountName: string;
  accountType: number;
  balance: string;
};

export type UpdateAccountForm = {
  id: number;
  accountName: string;
  accountType: number;
};

export const fetchAccountList = async () => {
  return await request("/api/Account/GetAccountList", {
    method: "GET",
  });
};

export const createAccount = async (params: CreateAccountForm) => {
  return await request("/api/Account/CreateAccount", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const updateAccount = async (params: UpdateAccountForm) => {
  return await request("/api/Account/UpdateAccount", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const deleteAccount = async (id: number) => {
  return await request("/api/Account/DeleteAccount", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ id }),
  });
};
