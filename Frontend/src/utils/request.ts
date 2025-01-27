import { notification } from "antd";
import * as process from "process";

export default async function request(url: string, options?: RequestInit) {
  const response = await fetch(url, options);
  if (!response.ok) {
    console.error("Fetch failed: HTTP " + response.statusText);
  }
  const responseJson = await response.json();
  if (responseJson) {
    if (responseJson.code === "0") {
      return responseJson.data;
    } else if (responseJson.code === "401") {
      window.location.href = process.env.contextPath + "/login";
    } else {
      notification.error({
        message: "操作失败",
        description: responseJson.message,
      });
    }
  } else {
    console.error("Fetch Failed: Empty response");
  }
}
