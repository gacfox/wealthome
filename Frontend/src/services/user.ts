import request from "@/utils/request";

export type User = {
  id: number;
  userName: string;
  displayName: string;
  email: string;
  avatarUrl: string;
  roleCode: string;
  roleName: string;
  status: number;
  statusName: string;
  createdAt: string;
  createdAtStr: string;
};

export type CreateUserForm = {
  userName: string;
  displayName: string;
  email: string;
  roleCode: string;
  password: string;
};

export type UpdateUserForm = {
  id: number;
  displayName: string;
  email: string;
  roleCode: string;
};

export const fetchUserList = async (queryDeleted: boolean) => {
  return await request("/api/User/QueryUserList", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      queryDeleted,
    }),
  });
};

export const createUser = async (params: CreateUserForm) => {
  return await request("/api/User/CreateUser", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const updateUser = async (params: UpdateUserForm) => {
  return await request("/api/User/UpdateUserInfo", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
};

export const doEnableUser = async (id: number) => {
  return await request("/api/User/EnableUser", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      id,
    }),
  });
};

export const doDisableUser = async (id: number) => {
  return await request("/api/User/DisableUser", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      id,
    }),
  });
};

export const doDeleteUser = async (id: number) => {
  return await request("/api/User/DeleteUser", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      id,
    }),
  });
};
