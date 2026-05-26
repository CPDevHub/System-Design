import { Component, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { LucideAngularModule, LayoutDashboard, Users, UserCog, FolderKanban, ListChecks, Settings, GaugeCircle, Sparkles, Briefcase, Clock, History, FilePlus2, LogOut, Search, Bell, ChevronDown } from 'lucide-angular';
import { AuthService } from '../../../core/services/auth.service';
import { AvatarComponent } from '../avatar/avatar.component';
import { StatusBadgeComponent } from '../status-badge/status-badge.component';

type Role = 'admin' | 'manager' | 'employee';

const NAV: Record<Role, { to: string; label: string; icon: any }[]> = {
  admin: [
    { to: '/admin/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { to: '/admin/users', label: 'Manage Users', icon: Users },
    { to: '/admin/employees', label: 'Manage Employees', icon: UserCog },
    { to: '/admin/projects', label: 'Manage Projects', icon: FolderKanban },
    { to: '/admin/allocations', label: 'All Allocations', icon: ListChecks },
    { to: '/admin/config', label: 'System Config', icon: Settings },
  ],
  manager: [
    { to: '/manager/resources', label: 'Resource Dashboard', icon: GaugeCircle },
    { to: '/manager/allocate', label: 'Allocate Resource', icon: Sparkles },
    { to: '/manager/projects', label: 'My Projects', icon: Briefcase },
    { to: '/manager/timesheets', label: 'Timesheets', icon: Clock },
    { to: '/manager/ai', label: 'AI Assistant', icon: Sparkles },
  ],
  employee: [
    { to: '/employee/timesheets/submit', label: 'Submit Timesheet', icon: FilePlus2 },
    { to: '/employee/allocations', label: 'My Allocations', icon: Briefcase },
    { to: '/employee/timesheets/history', label: 'Timesheet History', icon: History },
  ],
};

const USERS: Record<Role, { name: string; roleLabel: string }> = {
  admin:    { name: 'Asha Rao',     roleLabel: 'Admin' },
  manager:  { name: 'Marcus Lee',   roleLabel: 'Manager' },
  employee: { name: 'Elena Patel',  roleLabel: 'Employee' },
};

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, AvatarComponent, StatusBadgeComponent],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.css'
})
export class AppLayoutComponent {
  @Input() role: Role = 'admin';
  @Input() title: string = '';

  authService = inject(AuthService);
  router = inject(Router);

  get navItems() {
    return NAV[this.role] || [];
  }

  get user() {
    return USERS[this.role];
  }

  logout() {
    this.authService.logout();
  }
}
