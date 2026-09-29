export interface DailyBreakdownItem {
  date: string;
  collected: number;
  transactions: number;
}

export interface MethodBreakdownItem {
  method: string;
  amount: number;
  count: number;
}

export interface FeeCollectionSummaryDto {
  dateFrom: string;
  dateTo: string;
  totalCollected: number;
  totalTransactions: number;
  averageTransactionAmount: number;
  dailyBreakdown: DailyBreakdownItem[];
  methodBreakdown: MethodBreakdownItem[];
}

export interface DefaulterEntryDto {
  studentId: number;
  studentName: string;
  schoolClassName?: string | null;
  overdueInvoiceCount: number;
  totalOverdueAmount: number;
  oldestDueDate: string;
  daysOverdue: number;
}

export interface DefaulterReportDto {
  asOfDate: string;
  totalDefaulters: number;
  totalOverdueAmount: number;
  defaulters: DefaulterEntryDto[];
}
