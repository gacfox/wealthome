import { MonthStat, SumStat, UserExpenseSumStat } from "@/services/stat";
import * as echarts from "echarts";
import React, { useEffect, useRef } from "react";

const MonthStatChart: React.FC<{
  monthStatList: MonthStat[];
}> = ({ monthStatList }: { monthStatList: MonthStat[] }) => {
  const chartRef = useRef<HTMLDivElement | null>(null);
  const chartInstanceRef = useRef<echarts.ECharts | null>(null);

  const renderChart = () => {
    if (chartInstanceRef.current) {
      chartInstanceRef.current.setOption({
        title: {
          text: "家庭收支月度统计图",
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
          data: monthStatList?.map((item) => item.monthStr),
        },
        yAxis: {
          name: "金额（元）",
        },
        series: [
          {
            name: "家庭收入",
            type: "line",
            data: monthStatList?.map((item) => parseFloat(item.incomeStr)),
          },
          {
            name: "家庭支出",
            type: "line",
            data: monthStatList?.map((item) => parseFloat(item.expenseStr)),
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
    if (monthStatList && monthStatList.length) renderChart();
    window.addEventListener("resize", resizeChart);

    return () => {
      chartInstanceRef.current?.dispose();
      window.removeEventListener("resize", resizeChart);
    };
  }, []);

  useEffect(() => {
    if (monthStatList && monthStatList.length) renderChart();
  }, [monthStatList]);

  return <div style={{ height: "350px" }} ref={chartRef}></div>;
};

export default MonthStatChart;
