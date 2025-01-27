import { MonthStat, SumStat, UserExpenseSumStat } from "@/services/stat";
import * as echarts from "echarts";
import React, { useEffect, useRef } from "react";

const UserMonthStatChart: React.FC<{
  userMonthStatList: MonthStat[];
}> = ({ userMonthStatList }: { userMonthStatList: MonthStat[] }) => {
  const chartRef = useRef<HTMLDivElement | null>(null);
  const chartInstanceRef = useRef<echarts.ECharts | null>(null);

  const renderChart = () => {
    if (chartInstanceRef.current) {
      chartInstanceRef.current.setOption({
        title: {
          text: "个人收支月度统计图",
        },
        legend: {
          orient: "horizontal",
          x: "center",
          y: "bottom",
        },
        tooltip: {
          trigger: "axis",
        },
        xAxis: {
          name: "月度",
          data: userMonthStatList?.map((item) => item.monthStr),
        },
        yAxis: {
          name: "金额（元）",
        },
        series: [
          {
            name: "个人收入",
            type: "line",
            data: userMonthStatList?.map((item) => parseFloat(item.incomeStr)),
          },
          {
            name: "个人支出",
            type: "line",
            data: userMonthStatList?.map((item) => parseFloat(item.expenseStr)),
          },
        ],
      });
    }
  };

  const resizeChart = () => {
    if (chartInstanceRef.current) {
      chartInstanceRef.current.resize();
    }
  };

  useEffect(() => {
    chartInstanceRef.current = echarts.init(chartRef.current);
    if (userMonthStatList && userMonthStatList.length) renderChart();
    window.addEventListener("resize", resizeChart);

    return () => {
      chartInstanceRef.current?.dispose();
      window.removeEventListener("resize", resizeChart);
    };
  }, []);

  useEffect(() => {
    if (userMonthStatList && userMonthStatList.length) renderChart();
  }, [userMonthStatList]);

  return <div style={{ height: "350px" }} ref={chartRef}></div>;
};

export default UserMonthStatChart;
