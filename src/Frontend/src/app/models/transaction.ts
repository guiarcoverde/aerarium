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

export enum TransactionCategory {
  Salary = 100,
  Bonus = 101,
  Loan = 102,
  Investment = 103,
  ExtraIncome = 104,
  Gift = 105,
  Pix = 106,
  BankTransfer = 107,
  OtherIncome = 108,
  Housing = 200,
  Education = 201,
  Electronics = 202,
  Leisure = 203,
  OtherExpense = 204,
  Restaurant = 205,
  Health = 206,
  Services = 207,
  Grocery = 208,
  Transportation = 209,
  Clothing = 210,
  Travel = 211,
}

export enum SalaryScheduleMode {
  BusinessDay = 1,
  FixedDate = 2,
  FixedDateSplit = 3,
}

export interface CategoryOption {
  value: TransactionCategory;
  label: string;
}

export const INCOME_CATEGORIES: CategoryOption[] = [
  { value: TransactionCategory.Salary, label: 'Salary' },
  { value: TransactionCategory.Bonus, label: 'Bonus' },
  { value: TransactionCategory.Loan, label: 'Loan' },
  { value: TransactionCategory.Investment, label: 'Investment' },
  { value: TransactionCategory.ExtraIncome, label: 'Extra Income' },
  { value: TransactionCategory.Gift, label: 'Gift' },
  { value: TransactionCategory.Pix, label: 'Pix' },
  { value: TransactionCategory.BankTransfer, label: 'Bank Transfer' },
  { value: TransactionCategory.OtherIncome, label: 'Other Income' },
];

export const EXPENSE_CATEGORIES: CategoryOption[] = [
  { value: TransactionCategory.Housing, label: 'Housing' },
  { value: TransactionCategory.Education, label: 'Education' },
  { value: TransactionCategory.Electronics, label: 'Electronics' },
  { value: TransactionCategory.Leisure, label: 'Leisure' },
  { value: TransactionCategory.OtherExpense, label: 'Other Expense' },
  { value: TransactionCategory.Restaurant, label: 'Restaurant' },
  { value: TransactionCategory.Health, label: 'Health' },
  { value: TransactionCategory.Services, label: 'Services' },
  { value: TransactionCategory.Grocery, label: 'Grocery' },
  { value: TransactionCategory.Transportation, label: 'Transportation' },
  { value: TransactionCategory.Clothing, label: 'Clothing' },
  { value: TransactionCategory.Travel, label: 'Travel' },
];

export interface SalaryScheduleRequest {
  mode: SalaryScheduleMode;
  businessDayNumber?: number | null;
  fixedDay?: number | null;
  splitFirstAmount?: number | null;
  splitFirstPercentage?: number | null;
}

export interface CreateTransactionRequest {
  amount: number;
  description: string;
  date: string;
  type: TransactionType;
  category: TransactionCategory;
  recurrence: Recurrence;
  recurrenceEndDate?: string | null;
  recurrenceCount?: number | null;
  salarySchedule?: SalaryScheduleRequest | null;
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
