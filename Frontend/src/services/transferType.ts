import request from "@/utils/request";

export type TransferType = {
  id: number;
  name: string;
  inoutType: number;
  inoutTypeName: string;
};

export type CreateTransferTypeForm = {
  name: string;
  inoutType: number;
};

export type UpdateTransferTypeForm = {
  id: number;
  name: string;
  inoutType: number;
};

export const fetchTransferTypeList = async () => {
  return await request("/api/TransferType/GetTransferTypeList", {
    method: "GET",
  });
};

export const createTransferType = async (params: CreateTransferTypeForm) => {
  return await request("/api/TransferType/CreateTransferType", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const updateTransferType = async (params: UpdateTransferTypeForm) => {
  return await request("/api/TransferType/UpdateTransferType", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const deleteTransferType = async (id: number) => {
  return await request("/api/TransferType/DeleteTransferType", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ id }),
  });
};
