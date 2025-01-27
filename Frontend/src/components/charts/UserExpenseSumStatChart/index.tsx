import { SumStat, UserExpenseSumStat } from "@/services/stat";
import * as echarts from "echarts";
import React, { useEffect, useRef } from "react";

const UserExpenseSumStatChart: React.FC<{
  userExpenseSumStatList: UserExpenseSumStat[];
}> = ({
  userExpenseSumStatList,
}: {
  userExpenseSumStatList: UserExpenseSumStat[];
}) => {
  const chartRef = useRef<HTMLDivElement | null>(null);
  const chartInstanceRef = useRef<echarts.ECharts | null>(null);

  const renderChart = () => {
    if (chartInstanceRef.current) {
      chartInstanceRef.current.setOption({
        title: {
          text: "家庭成员收入统计图",
        },
        tooltip: {
          trigger: "item",
          formatter: (params: any) => {
            return `${params.name} ${parseFloat(params.value).toFixed(2)}`;
          },
        },
        legend: {
          orient: "vertical",
          left: "left",
          top: "15%",
        },
        series: [
          {
            name: "Access From",
            type: "pie",
            radius: ["40%", "70%"],
            avoidLabelOverlap: false,
            label: {
              show: false,
              position: "center",
            },
            emphasis: {
              label: {
                show: true,
                fontSize: 40,
                fontWeight: "bold",
              },
            },
            labelLine: {
              show: false,
            },
            data: userExpenseSumStatList?.map((item) => ({
              name: item.displayName,
              value: parseFloat(item.expenseStr),
            })),
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
    renderChart();
    window.addEventListener("resize", resizeChart);

    return () => {
      chartInstanceRef.current?.dispose();
      window.removeEventListener("resize", resizeChart);
    };
  }, []);

  useEffect(() => {
    renderChart();
  }, [userExpenseSumStatList]);

  return <div style={{ height: "200px" }} ref={chartRef}></div>;
};

export default UserExpenseSumStatChart;
