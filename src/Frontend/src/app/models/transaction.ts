export enum TransactionType {
  Income = 1,
  Expense = 2,
}

export enum Recurrence {
  None = 0,
  Daily = 1,
  Weekly = 2,
  Monthly = 3,
  Yearly = 4,
}

export interface CategoryBreakdownDto {
  category: number;
  categoryDisplayName: string;
  total: number;
  count: number;
}

export interface DashboardSummaryDto {
  totalIncome: number;
  totalExpenses: number;
  balance: number;
  incomeByCategory: CategoryBreakdownDto[];
  expenseByCategory: CategoryBreakdownDto[];
}
