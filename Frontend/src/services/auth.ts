import request from "@/utils/request";

export type LoginForm = {
  username: string;
  password: string;
};

export type InitAdminUserForm = {
  username: string;
  password: string;
};

export const doLogin = async (params: LoginForm) => {
  const response = await fetch("/api/Login/Login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
  return await response.json();
};

export const initAdminUser = async (params: InitAdminUserForm) => {
  const response = await fetch("/api/Login/InitAdminUser", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(params),
  });
  return await response.json();
};

export const doLogout = async () => {
  return await request("/api/Login/Logout", {
    method: "POST",
  });
};

export const fetchLoginInfo = async () => {
  return await request("/api/Login/GetLoginInfo", {
    method: "GET",
  });
};
