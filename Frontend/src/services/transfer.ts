import { TransferType } from "@/services/transferType";
import { User } from "@/services/user";
import { Account } from "@/services/account";
import request from "@/utils/request";

export type Flow = {
  id: number;
  accountId: number;
  accountDto: Account;
  amountStr: string;
  transferId: number;
  createdAt: string;
  createdAtStr: string;
};

export type Transfer = {
  id: number;
  title: string;
  transferTypeId: number;
  transferTypeDto: TransferType;
  description: string;
  userId: number;
  userDto: User;
  flowDtos: Flow[];
  createdAt: string;
  createdAtStr: string;
};

export type CreateTransferForm = {
  title: string;
  transferTypeId: number;
  description: string;
  fromAccountId: number;
  toAccountId: number;
  amountStr: string;
};

export type UpdateTransferForm = {
  id: number;
  title: string;
  transferTypeId: number;
  description: string;
};

export type QueryTransferForm = {
  pattern: string;
  transferUserId: number;
  transferTypeId: number;
  incomeAccountId: number;
  expenseAccountId: number;
  incomeUserId: number;
  expenseUserId: number;
  fromDate: string;
  toDate: string;
  pageNum: number;
  pageSize: number;
};

export const fetchTransferListByPage = async (params: QueryTransferForm) => {
  return await request("/api/Transfer/GetTransferListByPage", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const createTransfer = async (params: CreateTransferForm) => {
  return await request("/api/Transfer/CreateTransfer", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const revertTransfer = async (id: number) => {
  return await request("/api/Transfer/RevertTransfer", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ id }),
  });
};

export const updateTransfer = async (params: UpdateTransferForm) => {
  return await request("/api/Transfer/UpdateTransfer", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};
