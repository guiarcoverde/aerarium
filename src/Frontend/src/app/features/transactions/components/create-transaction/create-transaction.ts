import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { TransactionService } from '../../../../core/services/transaction.service';
import {
  CreateTransactionRequest,
  EXPENSE_CATEGORIES,
  INCOME_CATEGORIES,
  Recurrence,
  SalaryScheduleMode,
  TransactionCategory,
  TransactionType,
} from '../../../../models/transaction';

@Component({
  selector: 'app-create-transaction',
  imports: [ReactiveFormsModule],
  templateUrl: './create-transaction.html',
  styleUrl: './create-transaction.scss',
})
export class CreateTransaction {
  private readonly transactionService = inject(TransactionService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly submitting = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal(false);

  protected readonly form = new FormGroup({
    type: new FormControl<TransactionType | null>(null, Validators.required),
    category: new FormControl<TransactionCategory | null>(
      null,
      Validators.required,
    ),
    amount: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0.01),
    ]),
    description: new FormControl<string>('', [
      Validators.required,
      Validators.maxLength(500),
    ]),
    date: new FormControl<string>('', Validators.required),
    recurrence: new FormControl<Recurrence>(Recurrence.None, {
      nonNullable: true,
    }),
    recurrenceEndDate: new FormControl<string | null>(null),
    recurrenceCount: new FormControl<number | null>(null, Validators.min(1)),
    salaryScheduleMode: new FormControl<SalaryScheduleMode | null>(null),
    businessDayNumber: new FormControl<number | null>(null, [
      Validators.min(1),
      Validators.max(23),
    ]),
    fixedDay: new FormControl<number | null>(null, [
      Validators.min(1),
      Validators.max(31),
    ]),
    splitFirstAmount: new FormControl<number | null>(null, Validators.min(0.01)),
    splitFirstPercentage: new FormControl<number | null>(null, [
      Validators.min(0.01),
      Validators.max(100),
    ]),
  });

  private readonly typeValue = toSignal(this.form.controls.type.valueChanges);
  private readonly recurrenceValue = toSignal(
    this.form.controls.recurrence.valueChanges,
    { initialValue: Recurrence.None },
  );
  private readonly categoryValue = toSignal(
    this.form.controls.category.valueChanges,
  );
  private readonly salaryModeValue = toSignal(
    this.form.controls.salaryScheduleMode.valueChanges,
  );

  protected readonly filteredCategories = computed(() => {
    const type = this.typeValue();
    if (type === TransactionType.Income) return INCOME_CATEGORIES;
    if (type === TransactionType.Expense) return EXPENSE_CATEGORIES;
    return [];
  });

  protected readonly showRecurrenceFields = computed(
    () => this.recurrenceValue() !== Recurrence.None,
  );

  protected readonly showSalarySchedule = computed(
    () =>
      this.categoryValue() === TransactionCategory.Salary &&
      this.recurrenceValue() === Recurrence.Monthly,
  );

  protected readonly salaryMode = computed(() => this.salaryModeValue());

  protected readonly TransactionType = TransactionType;
  protected readonly Recurrence = Recurrence;
  protected readonly SalaryScheduleMode = SalaryScheduleMode;

  constructor() {
    this.form.controls.type.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.form.controls.category.reset();
      });

    this.form.controls.recurrence.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((val) => {
        if (val === Recurrence.None) {
          this.form.controls.recurrenceEndDate.reset();
          this.form.controls.recurrenceCount.reset();
        }
      });
  }

  protected onSubmit(): void {
    this.form.markAllAsTouched();
    this.error.set('');
    this.success.set(false);

    if (this.form.invalid) return;

    const f = this.form.getRawValue();
    if (
      f.recurrence !== Recurrence.None &&
      !f.recurrenceEndDate &&
      !f.recurrenceCount
    ) {
      this.error.set(
        'Provide either an end date or occurrence count for recurring transactions.',
      );
      return;
    }

    const request: CreateTransactionRequest = {
      amount: f.amount!,
      description: f.description!.trim(),
      date: f.date!,
      type: f.type!,
      category: f.category!,
      recurrence: f.recurrence,
      recurrenceEndDate:
        f.recurrence !== Recurrence.None ? (f.recurrenceEndDate ?? null) : null,
      recurrenceCount:
        f.recurrence !== Recurrence.None ? (f.recurrenceCount ?? null) : null,
      salarySchedule: this.showSalarySchedule()
        ? {
            mode: f.salaryScheduleMode!,
            businessDayNumber:
              f.salaryScheduleMode === SalaryScheduleMode.BusinessDay
                ? f.businessDayNumber
                : null,
            fixedDay:
              f.salaryScheduleMode === SalaryScheduleMode.FixedDate ||
              f.salaryScheduleMode === SalaryScheduleMode.FixedDateSplit
                ? f.fixedDay
                : null,
            splitFirstAmount:
              f.salaryScheduleMode === SalaryScheduleMode.FixedDateSplit
                ? f.splitFirstAmount
                : null,
            splitFirstPercentage:
              f.salaryScheduleMode === SalaryScheduleMode.FixedDateSplit
                ? f.splitFirstPercentage
                : null,
          }
        : null,
    };

    this.submitting.set(true);

    this.transactionService
      .create(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.success.set(true);
          this.form.reset({ recurrence: Recurrence.None });
        },
        error: (err) => {
          this.submitting.set(false);
          this.error.set(
            err.error?.detail ?? err.error?.title ?? 'Failed to create transaction.',
          );
        },
      });
  }
}
