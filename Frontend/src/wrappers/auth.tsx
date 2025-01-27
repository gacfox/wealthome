import { Outlet } from "umi";

function auth(props: any) {
  const loginInfoStr = localStorage.getItem("loginInfo");
  if (loginInfoStr) {
    const { isLogin, expireDate } = JSON.parse(loginInfoStr);
    if (isLogin && expireDate && new Date(expireDate).getTime() > Date.now()) {
      return <Outlet />;
    }
  }
  location.href = `${process.env.contextPath}/login`;
  return <></>;
}

export default auth;
