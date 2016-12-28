package y2015.R3.A

import cmn._

import scala.collection.mutable

object Fairland extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {
    abstract class ANode {
      val minSalary: Long
      val maxSalary: Long
    }

    case class BossNode(salary: Long) extends ANode {
      override val minSalary: Long = salary
      override val maxSalary: Long = salary
    }

    case class OtherNode(nodeManager: ANode, salary: Long) extends ANode {
      override lazy val minSalary: Long = Math.min(salary, nodeManager.minSalary)
      override lazy val maxSalary: Long = Math.max(salary, nodeManager.maxSalary)
    }


    val List(cNode, maxDiff) = fetch(PList(PLong))
    val List(s0, as, cs, rs) = fetch(PList(PLong))
    val List(m0, am, cm, rm) = fetch(PList(PLong))

    val rgnode = mutable.ArrayBuffer[ANode]()

    var s = s0
    var m = m0
    (0L until cNode).foreach(id => {
      if (id == 0)
        rgnode += BossNode(s)
      else {
        s = (s * as + cs) % rs
        m = (m * am + cm) % rm
        rgnode += OtherNode(rgnode((m % id).toInt), s)
      }
    })

    object Op extends Enumeration {
      val Start, End = Value
    }

    val rgx = rgnode.flatMap(node => {
      val start = node.maxSalary - maxDiff
      val end = node.minSalary

      if (end < start)
        Nil
      else
        List((start, Op.Start), (end, Op.End))
    }).sortWith(lt = {
      case ((num1, op1), (num2, op2)) => num1 < num2 || (num1 == num2 && op1 == Op.Start && op2 == Op.End)
    }).map(_._2)
      .toList

    def sums(enop: List[Op.Value], v: Long = 0) : Stream[Long] = enop match {
      case Nil => Stream()
      case op :: ops => {
        val dv = op match {
          case Op.Start => +1L
          case Op.End => -1L
        }
        (v+dv) #:: sums(ops, v+dv)
      }
    }

    out(sums(rgx).max)
  }
}
