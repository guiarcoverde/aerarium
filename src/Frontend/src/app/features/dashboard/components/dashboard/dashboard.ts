import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { DashboardSummaryDto, CategoryBreakdownDto } from '../../../../models/transaction';

@Component({
  selector: 'app-dashboard',
  imports: [CurrencyPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  protected readonly month = signal(new Date().getMonth() + 1);
  protected readonly year = signal(new Date().getFullYear());
  protected readonly summary = signal<DashboardSummaryDto | null>(null);
  protected readonly loading = signal(false);
  protected readonly error = signal('');

  private static readonly MONTH_NAMES = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December',
  ];

  ngOnInit(): void {
    this.loadSummary();
  }

  protected get monthName(): string {
    return Dashboard.MONTH_NAMES[this.month() - 1];
  }

  protected previousMonth(): void {
    if (this.month() === 1) {
      this.month.set(12);
      this.year.update((y) => y - 1);
    } else {
      this.month.update((m) => m - 1);
    }
    this.loadSummary();
  }

  protected nextMonth(): void {
    if (this.month() === 12) {
      this.month.set(1);
      this.year.update((y) => y + 1);
    } else {
      this.month.update((m) => m + 1);
    }
    this.loadSummary();
  }

  protected maxTotal(items: CategoryBreakdownDto[]): number {
    if (items.length === 0) return 1;
    return Math.max(...items.map((i) => i.total));
  }

  protected barWidth(item: CategoryBreakdownDto, items: CategoryBreakdownDto[]): number {
    const max = this.maxTotal(items);
    return max > 0 ? (item.total / max) * 100 : 0;
  }

  private loadSummary(): void {
    this.loading.set(true);
    this.error.set('');

    this.dashboardService.getSummary(this.month(), this.year()).subscribe({
      next: (data) => {
        this.summary.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.detail ?? 'Failed to load dashboard data');
        this.loading.set(false);
      },
    });
  }
}
